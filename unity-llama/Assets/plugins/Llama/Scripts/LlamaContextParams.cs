using System;
using System.Runtime.InteropServices;

namespace LlamaWrapper
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LlamaContextParams
    {
        public int seed;         // RNG seed, -1 for random
        public int n_ctx;        // text context
        public int n_batch;      // prompt processing batch size
        public int n_gqa;        // grouped-query attention (TEMP - will be moved to model hparams)
        public float rms_norm_eps; // rms norm epsilon (TEMP - will be moved to model hparams)
        public int n_gpu_layers; // number of layers to store in VRAM
        public int main_gpu;     // the GPU that is used for scratch and small tensors

        public IntPtr tensor_split; // how to split layers across multiple GPUs (size: LLAMA_MAX_DEVICES)

        // ref: https://github.com/ggerganov/llama.cpp/pull/2054
        public float rope_freq_base;  // RoPE base frequency
        public float rope_freq_scale; // RoPE frequency scaling factor

        // called with a progress value between 0 and 1, pass NULL to disable
        public IntPtr progress_callback;
        // context pointer passed to the progress callback
        public IntPtr progress_callback_user_data;

        // Keep the booleans together to avoid misalignment during copy-by-value.
        public byte low_vram;   // if true, reduce VRAM usage at the cost of performance
        public byte mul_mat_q;  // if true, use experimental mul_mat_q kernels
        public byte f16_kv;     // use fp16 for KV cache
        public byte logits_all; // the llama_eval() call computes all logits, not just the last one
        public byte vocab_only; // only load the vocabulary, no weights
        public byte use_mmap;   // use mmap if possible
        public byte use_mlock;  // force system to keep model in RAM
        public byte embedding;  // embedding mode only
    }
}
