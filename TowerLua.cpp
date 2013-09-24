#pragma once

extern "C"
{
    #include "lua.h"
	#include "lualib.h"
	#include "lauxlib.h"
	#include "luasocket.h"
	#include "mime.h"
}

void InitLuaSockets(lua_State *L)
{
	luaopen_socket_core(L);
}

void InitLuaMine(lua_State *L)
{
	luaopen_mime_core(L);
}