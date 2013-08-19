// Copyright Daniel Wallin 2008. Use, modification and distribution is
// subject to the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)

#ifndef LUABIND_CONST_VAR_081014_HPP
# define LUABIND_CONST_VAR_081014_HPP

#include <luabind/scope.hpp>
#include <luabind/HelpContainer.h>

namespace luabind {

namespace detail
{
  template <typename C>
  struct const_registration : registration
  {
	  const_registration(char const* name, C value)
		: name(name)
		, value(value)
	  {}

	  void register_(lua_State* L) const
	  {
		  object(from_stack(L, -1))[name.c_str()] = value;
		  if (!TowerLua::HelpContainer::enabled) return;

		  TowerLua::HelpContainer::AddInfo("CONST", name.c_str(), value, typeid(C).name());
	  }

	  std::string name;
	  C value;
  };

  template <typename C>
  struct var_registration : registration
  {
	  var_registration(char const* name, C value)
		: name(name)
		, value(value)
	  {}

	  void register_(lua_State* L) const
	  {
		  object(from_stack(L, -1))[name.c_str()] = value;
		  if (!TowerLua::HelpContainer::enabled) return;

		  TowerLua::HelpContainer::AddInfo("VAR", name.c_str(), value, typeid(C).name());
	  }

	  std::string name;
	  C value;
  };

} // namespace detail

template <typename F>
scope def_const(char const* name, F f)
{
	return scope(std::auto_ptr<detail::registration>(new detail::const_registration<F>(name, f)));
}

template <typename F>
scope def_var(char const* name, F f)
{
	return scope(std::auto_ptr<detail::registration>(new detail::var_registration<F>(name, f)));
}

} // namespace luabind

#endif // LUABIND_FUNCTION2_081014_HPP

