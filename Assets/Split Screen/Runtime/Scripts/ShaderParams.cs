using System;
using UnityEngine;

namespace SplitScreenPro {

    static class ShaderParams {
        public static int mainTex = Shader.PropertyToID("_MainTex");
        public static int inputTex = Shader.PropertyToID("_InputTex");
        public static int compareParams = Shader.PropertyToID("_CompareParams");
        public static int transitionParams = Shader.PropertyToID("_TransitionData");
        public static int source1 = Shader.PropertyToID("_Source1");
        public static int source2 = Shader.PropertyToID("_Source2");
        public static int splitLineColor = Shader.PropertyToID("_SplitLineColor");
        public static int splitLineCenter = Shader.PropertyToID("_SplitLineCenter");
        public static int smoothBorders = Shader.PropertyToID("_SmoothBorders");
    }

}