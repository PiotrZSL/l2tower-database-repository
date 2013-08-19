// Copyright (c) 2003 Daniel Wallin and Arvid Norberg

// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF
// ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT
// SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE
// OR OTHER DEALINGS IN THE SOFTWARE.

#define LUABIND_BUILDING
#include <luabind/lua_include.hpp>
#include <luabind/error.hpp>

namespace luabind
{
	namespace
	{
		pcall_callback_fun pcall_callback = 0;
#ifdef LUABIND_NO_EXCEPTIONS
		error_callback_fun error_callback = 0;
		cast_failed_callback_fun cast_failed_callback = 0;
#endif
	}

#ifndef LUABIND_NO_EXCEPTIONS
error::error(lua_State* L): m_L(L), m_error("Lua error: ")
{
	const char* tmp = lua_tostring(L,-1);
	if (tmp!=NULL)
	{
		m_error+=tmp;
	}
	else
	{
		m_error+="Unknown";
	}
}

cast_failed::cast_failed(lua_State* L, type_id const& i): m_L(L), m_info(i), m_error("Cast failed : ")
{
	m_error+=m_info.name();
	const char* tmp = lua_tostring(L,-1);
	if (tmp!=NULL)
	{
		m_error+=", Stack error: ";
		m_error+=tmp;
	}
}
#endif

#ifdef LUABIND_NO_EXCEPTIONS

	typedef void(*error_callback_fun)(lua_State*);
	typedef void(*cast_failed_callback_fun)(lua_State*, type_id const&);

	void set_error_callback(error_callback_fun e)
	{
		error_callback = e;
	}

	void set_cast_failed_callback(cast_failed_callback_fun c)
	{
		cast_failed_callback = c;
	}

	error_callback_fun get_error_callback()
	{
		return error_callback;
	}

	cast_failed_callback_fun get_cast_failed_callback()
	{
		return cast_failed_callback;
	}

#endif

	void set_pcall_callback(pcall_callback_fun e)
	{
		pcall_callback = e;
	}

	pcall_callback_fun get_pcall_callback()
	{
		return pcall_callback;
	}
}