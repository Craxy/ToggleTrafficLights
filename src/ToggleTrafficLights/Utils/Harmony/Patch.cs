using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Harmony.ILCopying;

namespace Harmony
{
  public sealed class Patch : IDisposable
  {
    private MethodBase _original;
    private MethodInfo _postfix;
    private PatchHandle _patchHandle;
    public bool IsPatched => _patchHandle != null && !_patchHandle.IsDisposed;

    private Patch(MethodBase original, MethodInfo postfix, PatchHandle patchHandle)
    {
      _original = original;
      _postfix = postfix;
      _patchHandle = patchHandle;
    }

    /// <returns>Must be kept alive</returns>
    public static Patch Apply(MethodInfo original, MethodInfo postfix)
    {
      return new Patch(original, postfix, Apply((MethodBase) original, postfix));
    }

    public void Restore()
    {
      if (IsPatched)
      {
        Restore(_original, _patchHandle);
      }
      Debug.Assert(!IsPatched);
      
      _patchHandle = null;
      
      _original = null;
      _postfix = null;
    }

    public void Dispose()
    {
      Restore();
    }

    private class PatchHandle : IDisposable
    {
      public DynamicMethod PatchedMethod { get; private set; }
      public byte[] OverwrittenCode { get; private set; }

      public PatchHandle(DynamicMethod patchedMethod, byte[] overwrittenCode)
      {
        PatchedMethod = patchedMethod ?? throw new ArgumentNullException(nameof(patchedMethod));
        OverwrittenCode = overwrittenCode ?? throw new ArgumentNullException(nameof(overwrittenCode));
      }

      public void Destroy() 
        => Dispose();
      public void Dispose()
      {
        PatchedMethod = null;
        OverwrittenCode = null;
      }

      public bool IsDisposed => PatchedMethod == null;
    }
    private static PatchHandle Apply(MethodBase original, MethodInfo postfix)
    {
      if (postfix == null)
      {
        throw new ArgumentNullException(nameof(postfix));
      }
			
      var originalCodeStart = Memory.GetMethodStart(original);

      var replacement = MethodPatcher.CreatePatchedMethod(original, postfix);
      if (replacement == null)
      {
        throw new MissingMethodException($"Cannot create dynamic replacement for {original}");
      }
      var patchCodeStart = Memory.GetMethodStart(replacement);
			
      // This part effectively corrupts the original compiled method, so we should prepare to restore the overwritten part later
      // (It doesn't look like it breaks something, but... better safe than sorry?)
      var oldBytes = new byte[IntPtr.Size == sizeof(long) ? 12 : 6];
      Marshal.Copy((IntPtr)originalCodeStart, oldBytes, 0, oldBytes.Length);

      Memory.WriteJump(originalCodeStart, patchCodeStart);

      return new PatchHandle(patchedMethod: replacement, overwrittenCode: oldBytes);
    }
    private static void Restore(MethodBase original, PatchHandle patchHandle)
    {
      if (patchHandle == null)
      {
        throw new ArgumentNullException(nameof(patchHandle));
      }
      if (patchHandle.IsDisposed)
      {
        throw new InvalidOperationException($"{nameof(patchHandle)} is disposed!");
      }
			
      var originalCodeStart = Memory.GetMethodStart(original);
      Memory.WriteBytes(originalCodeStart, patchHandle.OverwrittenCode);
			
      patchHandle.Dispose();
    }
  }
}
