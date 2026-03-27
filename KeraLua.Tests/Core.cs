
using System;
using NUnit.Framework;
using System.IO;
using KeraLua;

#if MONOTOUCH
	using ObjCRuntime;
#endif


namespace KeraLua.Tests
{
	[TestFixture]
	public class Core
	{
		public static readonly char UnicodeChar = '\uE007';
		public static string UnicodeString
		{
			get
			{
				return Convert.ToString (UnicodeChar);
			}
		}
		
#if MONOTOUCH
		[MonoPInvokeCallback (typeof (LuaNativeFunction))]
#endif
		static int print (LuaState state)
		{
			IntPtr L = state;
			int n = Lua.LuaGetTop(L);  /* number of arguments */
			int i;
			uint len;
			for (i=1; i<=n; i++) {
				int type = Lua.LuaType(L, i);
				switch (type) {
						
					case 0:
						Console.Write("nil");
						break;
					case 4:
						string s = Lua.LuaToLString (L, i, out len).ToString ((int)len);
						Console.Write(s);
						break;
					case 3:
						double number = Lua.LuaNetToNumber (L, i);
						Console.Write (number);
						break;
				}
			}
			Console.WriteLine();
			return 0;
		}

		static int TestUnicodeString (LuaState state)
		{
			uint len;
			CharPtr ptrParam = Lua.LuaToLString (state, 1, out len);
			string param = ptrParam.ToString ();

            Assert.That(param, Is.EqualTo(Core.UnicodeString), "#1 ToString()");

			string param2 = ptrParam.ToString ((int)len);

            Assert.That(param2, Is.EqualTo(Core.UnicodeString), "#2 ToString(len)");
			return 0;
		}

		static int TestWrappedCFunctionSuccess(LuaState state)
		{
			Lua.LuaPushString(state, "OK from TestWrappedCFunctionSuccess");
			return 1;
		}
		
		static int TestWrappedCFunctionFailure(LuaState state)
		{
			Lua.LuaPushString(state, "Error from TestWrappedCFunctionFailure");
			return -1;
		}

		public static LuaNativeFunction func_print = print;
		public static LuaNativeFunction FuncTestUnicodeString = TestUnicodeString;
		public static LuaNativeFunction FuncTestWrappedCFunctionSuccess = TestWrappedCFunctionSuccess;
		public static LuaNativeFunction FuncTestWrappedCFunctionFailure = TestWrappedCFunctionFailure;

		

		LuaState state;
		string GetTestPath(string name)
		{
			string filePath = Path.Combine ( "core", name + ".lua");
			return filePath;
		}

		void AssertString (string chunk)
		{
			string error = string.Empty;

			int result = Lua.LuaNetLoadBuffer (state, chunk, 0, "chunk");

			if (result != 0) {
				uint len;
				error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
			}

            Assert.That(result, Is.EqualTo(0), "Fail loading string: " + chunk + "ERROR:" + error);

			result = Lua.LuaNetPCall (state, 0, -1, 0);

			if (result != 0) {
				uint len;
				error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
			}
            Assert.That(result, Is.EqualTo(0), "Fail calling chunk: " + chunk + " ERROR: " + error);
		}

		void AssertFile (string path)
		{
			string error = string.Empty;

			int result = Lua.LuaNetLoadFile (state, path);

			if (result != 0) {
				uint len;
				error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
			}

            Assert.That(result, Is.EqualTo(0), "Fail loading file: " + path + "ERROR:" + error);
			
			result =  Lua.LuaNetPCall (state, 0, -1, 0);

			if (result != 0) {
				uint len;
				error = Lua.LuaToLString (state, 1, out len).ToString((int)len);
			}


            Assert.That(result, Is.EqualTo(0), "Fail calling file: " + path + " ERROR: " + error);
		}

		void TestLuaFile (string name)
		{
			string path = GetTestPath (name);
			AssertFile (path);
		}


		public void Setup()
		{
			state = Lua.LuaLNewState ();
			Lua.LuaLOpenLibs (state);
			Lua.LuaPushStdCallCFunction (state,  Core.func_print);
			Lua.LuaNetSetGlobal (state, "print");			
		}

		public void TearDown ()
		{
			Lua.LuaClose (state);
			state = IntPtr.Zero;
		}

		[Test]
		public void Bisect ()
		{
			Setup();
			TestLuaFile ("bisect");
			TearDown ();
		}

		[Test]
		public void CF ()
		{
			Setup ();
			TestLuaFile ("cf");
			TearDown ();
		}

		[Test]
		[Ignore("testFileMissing")]
		public void Env ()
		{
			Setup ();
			TestLuaFile ("env");
			TearDown ();
		}

		[Test]
		public void Factorial ()
		{
			Setup ();
			TestLuaFile ("factorial");
			TearDown ();
		}

		[Test]
		public void FibFor ()
		{
			Setup ();
			TestLuaFile ("fibfor");
			TearDown ();
		}

		[Test]
		public void Life ()
		{
			Setup ();
			TestLuaFile ("life");
			TearDown ();
		}

		[Test]
		public void Printf ()
		{
			Setup ();
			TestLuaFile ("printf");
			TearDown ();
		}

		[Test]
		[Ignore("testFileMissing")]
		public void ReadOnly ()
		{
			Setup ();
			TestLuaFile ("readonly");
			TearDown ();

		}

		[Test]
		public void Sieve ()
		{
			Setup ();
			TestLuaFile ("sieve");
			TearDown ();
		}

		[Test]
		public void Sort ()
		{
			Setup ();
			TestLuaFile ("sort");
			TearDown ();
		}

		[Test]
		[Ignore("testFileMissing")]
		public void TraceGlobals ()
		{
			Setup ();
			TestLuaFile ("trace-globals");
			TearDown ();
		}

		[Test]
		public void TestUnicodeString ()
		{
			Setup ();
			Lua.LuaPushStdCallCFunction (state, Core.FuncTestUnicodeString);
			Lua.LuaNetSetGlobal (state, "TestUnicodeString");
			Lua.LuaPushString (state, Core.UnicodeString);
			Lua.LuaNetSetGlobal (state, "unicodeString");
			AssertString ("TestUnicodeString(unicodeString)");
			TearDown ();
		}
		
		[Test]
		public void PushWrappedCFunctionSuccess ()
		{
			Setup ();
			Lua.LuaPushWrappedCFunction (state, Core.FuncTestWrappedCFunctionSuccess);
			Lua.LuaNetSetGlobal (state, "TestWrappedCFunctionSuccess");
			uint len;
			var result = Lua.LuaNetLoadBuffer (state, "return TestWrappedCFunctionSuccess()", 0, "chunk");
			if (result != 0) {
				var error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
				Assert.Fail ($"Failed loading chunk ({result}): {error}");
			}
			result = Lua.LuaNetPCall (state, 0, 1, 0);
			if (result != 0) {
				var error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
				Assert.Fail ($"Failed to call chunk ({result}): {error}");
			}
			Assert.That(Lua.LuaIsString(state, -1) , Is.EqualTo(1), "Result should be a string");
			var strResult = Lua.LuaToLString (state, -1, out len).ToString ((int)len);
			Assert.That(strResult, Is.EqualTo("OK from TestWrappedCFunctionSuccess"), "Unexpected result from wrapped C function");
			TearDown ();
		}
		
		[Test]
		public void PushWrappedCFunctionFailure ()
		{
			Setup ();
			Lua.LuaPushWrappedCFunction (state, Core.FuncTestWrappedCFunctionFailure);
			Lua.LuaNetSetGlobal (state, "TestWrappedCFunctionFailure");
			uint len;
			string error;
			var result = Lua.LuaNetLoadBuffer (state, "return TestWrappedCFunctionFailure()", 0, "chunk");
			if (result != 0) {
				error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
				Assert.Fail ($"Failed loading chunk ({result}): {error}");
			}
			result = Lua.LuaNetPCall (state, 0, 1, 0);
			Assert.That(result, Is.EqualTo(2), "Expected result to correspond to LUA_ERRRUN");
			Assert.That(Lua.LuaIsString(state, -1) , Is.EqualTo(1), "Error should be a string");
			error = Lua.LuaToLString (state, 1, out len).ToString ((int)len);
			Assert.That(error, Is.EqualTo("Error from TestWrappedCFunctionFailure"), "Unexpected error from wrapped C function");
			TearDown ();
		}
	}
}
