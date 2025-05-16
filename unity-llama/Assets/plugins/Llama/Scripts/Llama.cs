using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace LlamaWrapper
{
    using llama_token = Int32;

    public class Llama
    {
#if UNITY_STANDALONE_WIN
        const string libraryName = "libllama.dll";
#elif UNITY_STANDALONE_LINUX
        const string libraryName = "libllama.so";
#elif UNITY_STANDALONE_OSX
        const string libraryName = "libllama.dylib";
#endif
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void llama_backend_init(bool numa);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void llama_backend_free();

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern LlamaContextParams llama_context_default_params();

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int llama_eval(IntPtr ctx, IntPtr tokens, int n_tokens, int n_past, int n_threads);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void llama_free(IntPtr ctx);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void llama_free_model(IntPtr model);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_init_from_file([MarshalAs(UnmanagedType.LPStr)] string path_model, LlamaContextParams parameters);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_load_model_from_file([MarshalAs(UnmanagedType.LPStr)] string path_model, LlamaContextParams parameters);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_new_context_with_model(IntPtr model, LlamaContextParams parameters);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int llama_tokenize(
            IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string text,
            llama_token[] tokens,
            int n_max_tokens,
            bool add_bos
        );

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int llama_tokenize_with_model(
            IntPtr model,
            [MarshalAs(UnmanagedType.LPStr)] string text,
            llama_token[] tokens,
            int n_max_tokens,
            bool add_bos
        );

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int llama_n_vocab(IntPtr ctx);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int llama_n_ctx(IntPtr ctx);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_get_logits(IntPtr ctx);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_token_to_str(IntPtr ctx, llama_token token);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern int llama_token_to_ptr(IntPtr ctx, llama_token token, IntPtr res);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_token_to_str_with_model(IntPtr model, llama_token token);

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern llama_token llama_token_bos();

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern llama_token llama_token_eos();

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void llama_tokenize_prompt(
            IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string prompt
        );

        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern IntPtr llama_token_to_words(
            IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string word,
            int length = 0,
            int n_threads = 1
        );

        public delegate void OnNewText([MarshalAs(UnmanagedType.LPStr)] string text);
        [DllImport(libraryName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void llama_token_to_parts(
            IntPtr ctx,
            [MarshalAs(UnmanagedType.LPStr)] string word,
            OnNewText cb = null,
            int length = 0,
            int n_threads = 1
        );

        static IntPtr ctx;
        static IntPtr m;

        public static void Initialize(string model, string prompt = "")
        {
            try
            {
                llama_backend_init(false);
                LlamaContextParams param = llama_context_default_params();
                m = llama_load_model_from_file(model, param);
                if (m == IntPtr.Zero)
                {
                    throw new Exception(string.Format("Error: failed to load model '{0}'\n", model));
                }

                ctx = llama_new_context_with_model(m, param);
                if (ctx == IntPtr.Zero)
                {
                    llama_free_model(m);
                    throw new Exception(string.Format("Error: failed to create context with model '{0}'\n", model));
                }
                llama_tokenize_prompt(ctx, prompt);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }
        }

        public static void Run(string word)
        {
            try
            {
                llama_token_to_parts(ctx, word, LlamaEventHandler.OnNewText, 500);
                //IntPtr ptr = llama_token_to_words(ctx, word, 500);
                //LlamaEventHandler.OnNewText(Marshal.PtrToStringUTF8(ptr));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message + "\n" + e.StackTrace);
            }
        }

        public static void Destroy()
        {
            if (ctx != IntPtr.Zero)
            {
                llama_free(ctx);
            }
            if (m != IntPtr.Zero)
            {
                llama_free_model(m);
            }
            llama_backend_free();
        }
    }
}
