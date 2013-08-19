#pragma once

extern "C"
{
    #include "lua.h"
	#include "lualib.h"
	#include "lauxlib.h"
}

#include <luabind/luabind.hpp>
#include <luabind/function.hpp>
#include <luabind/object.hpp>
#include <luabind/operator.hpp>
#include <luabind/adopt_policy.hpp>
#include <luabind/iterator_policy.hpp>
#include <luabind/out_value_policy.hpp>
#include <MyGUI.h>

namespace luabind
{
	template <>
	struct default_converter<MyGUI::UString>
	  : native_converter_base<MyGUI::UString>
	{
		static int compute_score(lua_State* L, int index)
		{
			return lua_type(L, index) == LUA_TSTRING ? 0 : -1;
		}

		MyGUI::UString from(lua_State* L, int index)
		{
			return MyGUI::UString(lua_tostring(L, index), lua_strlen(L, index));
		}

		void to(lua_State* L, MyGUI::UString const& value)
		{
			lua_pushlstring(L, value.asUTF8_c_str(), value.size());
		}
	};

	template <>
	struct default_converter<MyGUI::UString const>
	  : default_converter<MyGUI::UString>
	{};

	template <>
	struct default_converter<MyGUI::UString const&>
	  : default_converter<MyGUI::UString>
	{};
};

inline void InitLuaState(lua_State *L)
{
	luaL_openlibs(L);
	luabind::open(L);
}