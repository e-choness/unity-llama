using System;
using System.Runtime.InteropServices;

namespace LlamaWrapper
{
    using llama_token = Int32;

    [StructLayout(LayoutKind.Sequential)]
    public struct LlamaTokenData
    {
        public llama_token id; // token id
        public float logit;    // log-odds of the token
        public float p;        // probability of the token
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LlamaTokenDataArray
    {
        public IntPtr data;
        public int size;
        public bool sorted;
    }
}
