using System;
using Microsoft.Xna.Framework;

// http://blog.roboblob.com/2013/07/27/solving-resolution-independent-rendering-and-2d-camera-using-monogame/comment-page-1/
namespace ToyWalrus.Rendering {
    class Camera {
        public static Camera Main { get; private set; }

        private Vector2 position;
        private float rotation;
        private float zoom;

        private bool isViewTransformationDirty = true;
        private Matrix transform = Matrix.Identity;
        private Matrix camTranslationMatrix = Matrix.Identity;
        private Matrix camRotationMatrix = Matrix.Identity;
        private Matrix camScaleMatrix = Matrix.Identity;
        private Matrix resTranslationMatrix = Matrix.Identity;

        private Vector3 camTranslationVector = Vector3.Zero;
        private Vector3 camScaleVector = Vector3.Zero;
        private Vector3 resTranslationVector = Vector3.Zero;

        public ResolutionIndependentRenderer ResolutionIndependentRenderer { get; protected set; }

        public Camera(ResolutionIndependentRenderer resolutionIndependentRenderer, bool isMainCamera = false) {
            ResolutionIndependentRenderer = resolutionIndependentRenderer;

            position = new Vector2(resolutionIndependentRenderer.VirtualWidth / 2, resolutionIndependentRenderer.VirtualHeight / 2);
            rotation = 0f;
            zoom = 1f;

            if (isMainCamera) {
                if (Main != null) {
                    throw new Exception("Cannot have more than 1 main camera!");
                }
                Main = this;
            }
        }

        public Vector2 Position {
            get => position;
            protected set {
                //if (position != value) {
                //    Utils.Logger.LogToConsole(value);
                //}
                position = value;
                isViewTransformationDirty = true;
            }
        }

        public float Zoom {
            get => zoom;
            set {
                zoom = MathHelper.Clamp(value, .1f, 3f);
                isViewTransformationDirty = true;
            }
        }

        public float Rotation {
            get => rotation;
            protected set {
                rotation = value;
                isViewTransformationDirty = true;
            }
        }

        public Matrix ViewTransformMatrix {
            get {
                if (isViewTransformationDirty) {
                    camTranslationVector.X = -position.X;
                    camTranslationVector.Y = -position.Y;
                    Matrix.CreateTranslation(ref camTranslationVector, out camTranslationMatrix);

                    Matrix.CreateRotationZ(rotation, out camRotationMatrix);

                    camScaleVector.X = zoom;
                    camScaleVector.Y = zoom;
                    camScaleVector.Z = 1;
                    Matrix.CreateScale(ref camScaleVector, out camScaleMatrix);

                    resTranslationVector.X = ResolutionIndependentRenderer.VirtualWidth * .5f;
                    resTranslationVector.Y = ResolutionIndependentRenderer.VirtualHeight * .5f;
                    resTranslationVector.Z = 0;
                    Matrix.CreateTranslation(ref resTranslationVector, out resTranslationMatrix);

                    transform =
                        camTranslationMatrix *
                        camRotationMatrix *
                        camScaleMatrix;// *
                        //resTranslationMatrix *
                        //ResolutionIndependentRenderer.GetTransformationMatrix();

                    isViewTransformationDirty = false;
                }
                return transform;
            }
        }

        public void InitializeResolutionIndependence(int screenWidth, int screenHeight) {
            //var rir = ResolutionIndependentRenderer;
            //rir.ScreenWidth = screenWidth;
            //rir.ScreenHeight = screenHeight;
            //rir.Initialize();

            RecalculateTransformationMatrices();
        }

        public void BeforeDraw() {
            ResolutionIndependentRenderer.BeginDraw();
        }

        public void RecalculateTransformationMatrices() {
            isViewTransformationDirty = true;
        }

        public void Move(Vector2 offset) {
            SetPosition(Position + offset);
        }

        public void SetPosition(Vector2 newPosition) {
            Position = newPosition;
        }

        public Vector2 ScreenPointToWorldPoint(Vector2 screenPoint) {
            return Vector2.Transform(screenPoint, Matrix.Invert(ViewTransformMatrix));
        }
    }
}
