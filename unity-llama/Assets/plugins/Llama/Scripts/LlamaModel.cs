using System;

namespace LlamaWrapper
{
    public enum EModel
    {
        MODEL_UNKNOWN,
        MODEL_3B,
        MODEL_7B,
        MODEL_13B,
        MODEL_30B,
        MODEL_65B,
        MODEL_70B,
    }

    public struct LlamaModel
    {
        public EModel type;

        public int n_gpu_layers;

        public int t_load_us;
        public int t_start_us;
    }
}
