﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace KeraLua
{
    static class NativeMethods
    {
#if MONOTOUCH
		const string LIBNAME = "__Internal";
#else
#if DEBUGLUA
		const string LIBNAME = "lua52d";
#else
        const string LIBNAME = "lua52";
#endif

#if USE_DYNAMIC_DLL_REGISTER
        static NativeMethods()
        {
            NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, DllImportResolver);
        }

        private static string GetSearchPatternOs(bool isWindows, bool isLinux)
        {
            return isWindows ? "win" : isLinux ? "linux" : "";
        }

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr handle;
            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle))
            {
                return handle;
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                libraryName = $"lib{libraryName}.so";
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                libraryName = $"{libraryName}.dll";
            }

            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out handle))
            {
                return handle;
            }
           
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (NativeLibrary.TryLoad(libraryName, assembly, DllImportSearchPath.ApplicationDirectory, out handle))
                {
                    return handle;
                }

                Console.WriteLine($"try loading from AssemblyDirectory");
                if (NativeLibrary.TryLoad(libraryName, assembly, DllImportSearchPath.AssemblyDirectory, out handle))
                {
                    return handle;
                }
            }
            
            var runtimePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes");
            var preferredPath = Path.Combine(runtimePath, RuntimeInformation.RuntimeIdentifier, "native");
            if (Directory.Exists(preferredPath))
            {
                Console.WriteLine($"try loading preferred {Path.Combine(preferredPath, libraryName)}");
                if (NativeLibrary.TryLoad(Path.Combine(preferredPath, libraryName), assembly, searchPath, out handle))
                {
                    return handle;
                }
            }

            var architecture = RuntimeInformation.RuntimeIdentifier.Split('-').Last();
            var searchPattern =
                $"{GetSearchPatternOs(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), RuntimeInformation.IsOSPlatform(OSPlatform.Linux))}*{architecture}";
            Console.WriteLine($"searchPattern {searchPattern}");
            foreach (var directory in Directory.GetDirectories(runtimePath, searchPattern, SearchOption.AllDirectories))
            {
                var nativeDir = Path.Combine(directory, "native");
                if (!Directory.Exists(nativeDir))
                    continue;

                Console.WriteLine($"try loading {Path.Combine(nativeDir, libraryName)}");
                if (NativeLibrary.TryLoad(Path.Combine(nativeDir, libraryName), assembly, searchPath, out handle))
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }
#endif

#endif

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gc")]
        internal static extern int LuaGC(IntPtr luaState, int what, int data);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_typename")]
        internal static extern IntPtr LuaTypeName(IntPtr luaState, int type);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luaL_error")]
        internal static extern void LuaLError(IntPtr luaState, string message);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_where")]
        internal static extern void LuaLWhere(IntPtr luaState, int level);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_newstate")]
        internal static extern IntPtr LuaLNewState();

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_close")]
        internal static extern void LuaClose(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_openlibs")]
        internal static extern void LuaLOpenLibs(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luaL_loadstring")]
        internal static extern int LuaLLoadString(IntPtr luaState, string chunk);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_loadstring")]
        internal static extern int LuaLLoadString(IntPtr luaState, byte[] chunk);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_createtable")]
        internal static extern void LuaCreateTable(IntPtr luaState, int narr, int nrec);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gettable")]
        internal static extern void LuaGetTable(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_settop")]
        internal static extern void LuaSetTop(IntPtr luaState, int newTop);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_insert")]
        internal static extern void LuaInsert(IntPtr luaState, int newTop);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_remove")]
        internal static extern void LuaRemove(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_rawget")]
        internal static extern void LuaRawGet(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_settable")]
        internal static extern void LuaSetTable(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_rawset")]
        internal static extern void LuaRawSet(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setmetatable")]
        internal static extern void LuaSetMetatable(IntPtr luaState, int objIndex);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getmetatable")]
        internal static extern int LuaGetMetatable(IntPtr luaState, int objIndex);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_equal")]
        internal static extern int LuaNetEqual(IntPtr luaState, int index1, int index2);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushvalue")]
        internal static extern void LuaPushValue(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_replace")]
        internal static extern void LuaReplace(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gettop")]
        internal static extern int LuaGetTop(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_type")]
        internal static extern int LuaType(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_ref")]
        internal static extern int LuaLRef(IntPtr luaState, int registryIndex);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_rawgeti")]
        internal static extern void LuaRawGetI(IntPtr luaState, int tableIndex, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_rawseti")]
        internal static extern void LuaRawSetI(IntPtr luaState, int tableIndex, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_newuserdata")]
        internal static extern IntPtr LuaNewUserData(IntPtr luaState, uint size);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_touserdata")]
        internal static extern IntPtr LuaToUserData(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_unref")]
        internal static extern void LuaLUnref(IntPtr luaState, int registryIndex, int reference);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_isstring")]
        internal static extern int LuaIsString(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_isstring_strict")]
        internal static extern int LuaNetIsStringStrict(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_iscfunction")]
        internal static extern int LuaIsCFunction(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushnil")]
        internal static extern void LuaPushNil(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_pcall")]
        internal static extern int LuaNetPCall(IntPtr luaState, int nArgs, int nResults, int errfunc);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_tocfunction")]
        internal static extern IntPtr LuaToCFunction(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_tonumber")]
        internal static extern double LuaNetToNumber(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_toboolean")]
        internal static extern int LuaToBoolean(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_atpanic")]
        internal static extern void LuaAtPanic(IntPtr luaState, IntPtr panicf);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushstdcallcfunction")]
        internal static extern void LuaPushStdCallCFunction(IntPtr luaState, IntPtr function);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushnumber")]
        internal static extern void LuaPushNumber(IntPtr luaState, double number);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushboolean")]
        internal static extern void LuaPushBoolean(IntPtr luaState, int value);


        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_tolstring")]
        internal static extern IntPtr LuaToLString(IntPtr luaState, int index, out uint strLen);

        [SupportedOSPlatform("windows")]
		[DllImport (LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint =
 "luanet_pushlwstring")]
        internal static extern void LuaNetPushLWString(IntPtr luaState, string str, uint size);
        
        [UnsupportedOSPlatform("windows")]
        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_pushlstring")]
        internal static extern void LuaNetPushLString(IntPtr luaState, string str, uint size);

        [SupportedOSPlatform("windows")]
		[DllImport (LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint =
 "luanet_pushwstring")]
        internal static extern void LuaPushWString(IntPtr luaState, string str);
        
        [UnsupportedOSPlatform("windows")]
        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "lua_pushstring")]
        internal static extern void LuaPushString(IntPtr luaState, string str);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luaL_newmetatable")]
        internal static extern int LuaLNewMetatable(IntPtr luaState, string meta);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "lua_getfield")]
        internal static extern void LuaGetField(IntPtr luaState, int stackPos, string meta);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luaL_checkudata")]
        internal static extern IntPtr LuaLCheckUData(IntPtr luaState, int stackPos, string meta);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luaL_getmetafield")]
        internal static extern int LuaLGetMetafield(IntPtr luaState, int stackPos, string field);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_loadbuffer")]
        internal static extern int LuaNetLoadBuffer(IntPtr luaState, string buff, uint size, string name);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_loadbuffer")]
        internal static extern int LuaNetLoadBuffer(IntPtr luaState, byte[] buff, uint size, string name);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_loadfile")]
        internal static extern int LuaNetLoadFile(IntPtr luaState, string filename);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_error")]
        internal static extern void LuaError(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_checkstack")]
        internal static extern int LuaCheckStack(IntPtr luaState, int extra);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_next")]
        internal static extern int LuaNext(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_pushlightuserdata")]
        internal static extern void LuaPushLightUserData(IntPtr luaState, IntPtr udata);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luaL_checkmetatable")]
        internal static extern int LuaLCheckMetatable(IntPtr luaState, int obj);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gethookmask")]
        internal static extern int LuaGetHookMask(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_sethook")]
        internal static extern int LuaSetHook(IntPtr luaState, IntPtr func, int mask, int count);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_gethookcount")]
        internal static extern int LuaGetHookCount(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getinfo")]
        internal static extern int LuaGetInfo(IntPtr luaState, string what, IntPtr ar);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getstack")]
        internal static extern int LuaGetStack(IntPtr luaState, int level, IntPtr n);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getlocal")]
        internal static extern IntPtr LuaGetLocal(IntPtr luaState, IntPtr ar, int n);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setlocal")]
        internal static extern IntPtr LuaSetLocal(IntPtr luaState, IntPtr ar, int n);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_getupvalue")]
        internal static extern IntPtr LuaGetUpValue(IntPtr luaState, int funcindex, int n);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lua_setupvalue")]
        internal static extern IntPtr LuaSetUpValue(IntPtr luaState, int funcindex, int n);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_tonetobject")]
        internal static extern int LuaNetToNetObject(IntPtr luaState, int index);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_newudata")]
        internal static extern void LuaNetNewUData(IntPtr luaState, int val);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_rawnetobj")]
        internal static extern int LuaNetRawNetObj(IntPtr luaState, int obj);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_checkudata")]
        internal static extern int LuaNetCheckUData(IntPtr luaState, int ud, string tname);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_gettag")]
        internal static extern IntPtr LuaNetGetTag();

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_pushglobaltable")]
        internal static extern void LuaNetPushGlobalTable(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_popglobaltable")]
        internal static extern void LuaNetPopGlobalTable(IntPtr luaState);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_getglobal")]
        internal static extern void LuaNetGetGlobal(IntPtr luaState, string name);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi,
            EntryPoint = "luanet_setglobal")]
        internal static extern void LuaNetSetGlobal(IntPtr luaState, string name);

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_registryindex")]
        internal static extern int LuaNetRegistryIndex();

        [DllImport(LIBNAME, CallingConvention = CallingConvention.Cdecl, EntryPoint = "luanet_get_main_state")]
        internal static extern IntPtr LuaNetGetMainState(IntPtr luaState);
    }
}