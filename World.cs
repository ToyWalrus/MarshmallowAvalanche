using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MarshmallowAvalanche.Utils;

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

        private Dictionary<MovingObject, List<WorldGridSection>> blockSections;
        private IEnumerable<MovingObject> Blocks => blockSections.Keys;
        private readonly WorldGrid grid;

        public World(int width, int height) {
            _width = width;
            _height = height;
            blockSections = new Dictionary<MovingObject, List<WorldGridSection>>();

            grid = new WorldGrid(this);
        }

        public List<MovingObject> GetBlocks() {
            return new List<MovingObject>(Blocks);
        }

        public void Update(GameTime gt) {
            foreach (MovingObject block in Blocks) {
                block.Update(gt);
                UpdateContainingSection(block);
                CheckCollisions();
            }
        }

        private void CheckCollisions() {
            List<WorldGridSection> sections = grid.GetSections();
            for (int i = 0; i < sections.Count; ++i) {
                List<MovingObject> objectsInSection = sections[i].containedObjects;
                for (int a = 0; a < objectsInSection.Count; ++a) {
                    MovingObject objA = objectsInSection[a];
                    for (int b = a + 1; b < objectsInSection.Count - 1; ++b) {
                        MovingObject objB = objectsInSection[b];

                        Vector2 overlap;
                        if (objA.Bounds.Intersects(objB.Bounds, out overlap)) {
                            objA.AddCollision(new CollisionData(objB, overlap, objA.Velocity, objB.Velocity, objA.PreviousPosition, objB.PreviousPosition, objA.Position, objB.Position));
                            objB.AddCollision(new CollisionData(objA, -overlap, objB.Velocity, objA.Velocity, objB.PreviousPosition, objA.PreviousPosition, objB.Position, objA.Position));
                        }
                    }
                }
            }
        } 

        private void UpdateContainingSection(MovingObject obj) {
            Rectangle bounds = obj.Bounds;
            WorldGridSection topLeftSection = grid.GetSectionAtPoint(bounds.Left, bounds.Top);
            WorldGridSection topRightSection = grid.GetSectionAtPoint(bounds.Right, bounds.Top);
            WorldGridSection botLeftSection = grid.GetSectionAtPoint(bounds.Left, bounds.Bottom);
            WorldGridSection botRightSection = grid.GetSectionAtPoint(bounds.Right, bounds.Bottom);

            for (int i = 0; i < blockSections[obj].Count; ++i) {
                blockSections[obj][i]?.RemoveObject(obj);
            }

            topLeftSection?.AddObject(obj);
            topRightSection?.AddObject(obj);
            botLeftSection?.AddObject(obj);
            botRightSection?.AddObject(obj);
        }
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
            private set {
                _cols = Math.Max(1, value);
                RecalculateGridSections();
            }
        }

        public float GridSectionHeight { get; private set; }
        public float GridSectionWidth { get; private set; }

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

        public WorldGridSection GetSectionAtPoint(Vector2 point) {
            int x = (int)point.X;
            int y = (int)point.Y;
            return GetSectionAtPoint(x, y);
        }

        public WorldGridSection GetSectionAtPoint(int x, int y) {
            int idx = (x * _cols) + y;

            if (idx < 0 || idx >= gridSections.Count) {
                return null;
            }

            return gridSections[idx];
        }

        public void RecalculateGridSections() {
            List<WorldGridSection> newGrid = new List<WorldGridSection>();
            GridSectionHeight = world.Height / (float)_rows;
            GridSectionWidth = world.Width / (float)_cols;

            for (int i = 0; i < _rows; ++i) {
                for (int j = 0; j < _cols; ++j) {
                    WorldGridSection newSection = new WorldGridSection {
                        id = newGrid.Count,
                        width = GridSectionWidth,
                        height = GridSectionHeight,
                    };

                    WorldGridSection oldSection = GetSectionAtPoint(j, i);
                    if (oldSection != null) {
                        newSection.containedObjects.AddRange(oldSection.containedObjects);
                    }

                    newGrid.Add(newSection);
                }
            }

            gridSections = newGrid;
        }
    }

    internal class WorldGridSection {
        public int id;
        public float width;
        public float height;
        public List<MovingObject> containedObjects = new List<MovingObject>();

        public void AddObject(MovingObject obj) {
            if (!containedObjects.Contains(obj)) {
                containedObjects.Add(obj);
            }
        }

        public void RemoveObject(MovingObject obj) {
            containedObjects.Remove(obj);
        }
    }
}
