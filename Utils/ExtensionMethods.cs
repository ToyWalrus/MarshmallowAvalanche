using System;
using System.Collections.Generic;
using System.Text;

namespace MarshmallowAvalanche.Utils {
    public static class ExtensionMethods {
        public static bool CloseEnough(this float self, float other, float epsilon = 0.1f) {
            return MathF.Abs(other - self) < epsilon;
        }

    }
}
