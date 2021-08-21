using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MarshmallowAvalanche.Utils;
using MarshmallowAvalanche.Physics;

namespace MarshmallowAvalanche {
    public class World {
        private int _width;
        public int Width {
            get => _width;
            set {
                _width = value;
                grid.RecalculateGridSections();
            }
        }

        private int _height;
        public int Height {
            get => _height;
            set {
                _height = value;
                grid.RecalculateGridSections();
            }
        }

        private readonly Dictionary<PhysicsObject, List<WorldGridSection>> objectSections;
        private readonly WorldGrid grid;

        public World(int width, int height) {
            _width = width;
            _height = height;

            objectSections = new Dictionary<PhysicsObject, List<WorldGridSection>>();
            grid = new WorldGrid(this);
        }

        public World(int width, int height, int numSubdividedColumns, int numSubdividedRows) : this(width, height) {
            grid.Columns = numSubdividedColumns;
            grid.Rows = numSubdividedRows;
        }

        public IEnumerable<PhysicsObject> GetPhysicsObjects() {
            return objectSections.Keys;
        }

        public void SpawnObject(PhysicsObject newObject) {
            List<WorldGridSection> sections = new List<WorldGridSection>();
            foreach (WorldGridSection section in grid.GetSectionsOverlappedBy(newObject.Bounds)) {
                section?.AddObject(newObject);
                sections.Add(section);
            }            
            objectSections.Add(newObject, sections);
        }

        public void RemoveObject(PhysicsObject physicsObject) {
            if (objectSections.ContainsKey(physicsObject)) {
                foreach (WorldGridSection section in objectSections[physicsObject]) {
                    section?.RemoveObject(physicsObject);
                }
                objectSections.Remove(physicsObject);
            }
        }

        public void Update(GameTime gt) {
            foreach (PhysicsObject obj in objectSections.Keys) {
                obj.Update(gt);
                UpdateCollisionDataFor(obj);
            }
        }

        private void UpdateCollisionDataFor(PhysicsObject obj) {
            if (obj.IsStatic && !(obj as StaticObject).hasSetCollisionBoundsInWorld) {
                UpdateContainingSection(obj);
                (obj as StaticObject).hasSetCollisionBoundsInWorld = true;
            } else if (obj.IsDynamic && (obj as MovingObject).Velocity.Length() > 0) {
                UpdateContainingSection(obj);
                (obj as MovingObject).ResetCollisionStatus();
            }

            foreach (WorldGridSection section in objectSections[obj]) {
                if (section == null) continue;
                List<PhysicsObject> objectsInSection = section.containedObjects;
                for (int a = 0; a < objectsInSection.Count; ++a) {
                    PhysicsObject other = objectsInSection[a];
                    if (obj == other) continue;
                    obj.CheckForCollisionWith(other);
                }
            }
        }

        private void UpdateContainingSection(PhysicsObject obj) {
            Rectangle bounds = obj.Bounds;
            List<WorldGridSection> objSections = objectSections[obj];
            for (int i = 0; i < objSections.Count; ++i) {
                objSections[i]?.RemoveObject(obj);
            }
            objSections.Clear();

            foreach (WorldGridSection section in grid.GetSectionsOverlappedBy(bounds)) {
                section?.AddObject(obj);
                objSections.Add(section);
            }
        }

        #region Debugging
        public void DrawGrid(SpriteBatch sb, float thickness = 1.5f) {
            List<WorldGridSection> sections = grid.GetSections();
            foreach (WorldGridSection section in sections) {
                Logger.DrawLine(sb, new Vector2(section.positionX, section.positionY), new Vector2(section.positionX, section.positionY + grid.GridSectionHeight), Color.Red, thickness);
                Logger.DrawLine(sb, new Vector2(section.positionX, section.positionY), new Vector2(section.positionX + grid.GridSectionWidth, section.positionY), Color.Red, thickness);

                foreach (PhysicsObject obj in section.containedObjects) {
                    if (obj is Character) {
                        Logger.DrawFilledRect(sb, new Rectangle(section.positionX, section.positionY, grid.GridSectionWidth, grid.GridSectionHeight), new Color(0, 255, 0, 150));
                        //break;
                    } else if (obj is StaticObject) {
                        Logger.DrawFilledRect(sb, new Rectangle(section.positionX, section.positionY, grid.GridSectionWidth, grid.GridSectionHeight), new Color(0, 0, 255, 150));
                    }
                }
            }
        }

        public void DrawAllSpawnedObjects(SpriteBatch sb) {
            DrawAllSpawnedObjects(sb, Color.OrangeRed, (p) => true);
        }

        public void DrawAllSpawnedObjects(SpriteBatch sb, Color color, Predicate<PhysicsObject> checkToDraw) {
            foreach (PhysicsObject obj in objectSections.Keys) {
                if (checkToDraw(obj)) {
                    Logger.DrawFilledRect(sb, obj.Bounds, color);
                }
            }
        }
        #endregion
    }

    internal class WorldGrid {
        public World world;
        private List<WorldGridSection> gridSections;

        private int _rows;
        public int Rows {
            get => _rows;
            set {
                _rows = Math.Max(1, value);
                RecalculateGridSections();
            }
        }

        private int _cols;
        public int Columns {
            get => _cols;
            set {
                _cols = Math.Max(1, value);
                RecalculateGridSections();
            }
        }

        public int GridSectionHeight { get; private set; }
        public int GridSectionWidth { get; private set; }

        public WorldGrid(World world) {
            this.world = world;
            gridSections = new List<WorldGridSection>();
            _rows = 1;
            _cols = 1;
            RecalculateGridSections();
        }

        public List<WorldGridSection> GetSections() {
            return gridSections;
        }

        public WorldGridSection GetSectionAtWorldPoint(Vector2 point) {
            return GetSectionAtWorldPoint((int)point.X, (int)point.Y);
        }

        public WorldGridSection GetSectionAtWorldPoint(int x, int y) {
            for (int i = 0; i < gridSections.Count; ++i) {
                int sectionMinX = gridSections[i].positionX;
                int sectionMinY = gridSections[i].positionY;
                int sectionMaxX = sectionMinX + GridSectionWidth;
                int sectionMaxY = sectionMinY + GridSectionHeight;

                if (x >= sectionMinX && x < sectionMaxX && y >= sectionMinY && y < sectionMaxY) {
                    return gridSections[i];
                }
            }

            return null;
        }

        public IEnumerable<WorldGridSection> GetSectionsOverlappedBy(Rectangle rectInWorld) {
            WorldGridSection topLeftSection = GetSectionAtWorldPoint(rectInWorld.Left, rectInWorld.Top);
            int offsetX, offsetY;

            if (topLeftSection != null) {
                offsetX = rectInWorld.Left - topLeftSection.positionX;
                offsetY = rectInWorld.Top - topLeftSection.positionY;
            } else {
                offsetX = 0;
                offsetY = 0;
            }

            for (int y = rectInWorld.Top - offsetY; y < rectInWorld.Bottom; y += GridSectionHeight) {
                for (int x = rectInWorld.Left - offsetX; x < rectInWorld.Right; x += GridSectionWidth) {
                    yield return GetSectionAtWorldPoint(x, y);
                }
            }
        }

        public WorldGridSection GetSectionAtGridPoint(Vector2 point) {
            return GetSectionAtGridPoint((int)point.X, (int)point.Y);
        }

        public WorldGridSection GetSectionAtGridPoint(int x, int y) {
            int idx = (x * _cols) + y;

            if (idx < 0 || idx >= gridSections.Count) {
                return null;
            }

            return gridSections[idx];
        }

        public void RecalculateGridSections() {
            List<WorldGridSection> newGrid = new List<WorldGridSection>();
            GridSectionHeight = world.Height / _rows;
            GridSectionWidth = world.Width / _cols;

            // Start at 0, because we never want the character
            // jumping off top of screen
            for (int i = 0; i < _rows + 2; ++i) {

                // Starting off left side screen and going
                // off right side of screen
                for (int j = -1; j < _cols + 1; ++j) {

                    WorldGridSection newSection = new WorldGridSection {
                        id = newGrid.Count,
                        width = GridSectionWidth,
                        height = GridSectionHeight,
                        positionX = j * GridSectionWidth,
                        positionY = i * GridSectionHeight,
                    };

                    //WorldGridSection oldSection = GetSectionAtGridPoint(j, i);
                    //if (oldSection != null) {
                    //    newSection.containedObjects.AddRange(oldSection.containedObjects);
                    //}

                    newGrid.Add(newSection);
                }
            }

            gridSections = newGrid;
        }
    }

    internal class WorldGridSection {
        public int id;
        public int width;
        public int height;
        public int positionX;
        public int positionY;

        public List<PhysicsObject> containedObjects = new List<PhysicsObject>();

        public void AddObject(PhysicsObject obj) {
            if (!containedObjects.Contains(obj)) {
                containedObjects.Add(obj);
            }
        }

        public void RemoveObject(PhysicsObject obj) {
            containedObjects.Remove(obj);
        }
    }
}
