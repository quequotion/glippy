dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([glippy], [0.6.0.0])
AM_INIT_AUTOMAKE([foreign])
AM_MAINTAINER_MODE
AM_GCONF_SOURCE_2

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

m4_include([expansions.m4])
SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

AC_PROG_INSTALL

AC_PATH_PROG(DMCS, dmcs, no)
if test "x$DMCS" = "xno"; then
        AC_MSG_ERROR([dmcs Not found])
fi


AC_ARG_ENABLE(debug_x86,
	AC_HELP_STRING([--enable-debug_x86],
		[Use 'DEBUG_X86' Configuration [default=NO]]),
		enable_debug_x86=yes, enable_debug_x86=no)
AM_CONDITIONAL(ENABLE_DEBUG_X86, test x$enable_debug_x86 = xyes)
if test "x$enable_debug_x86" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
AC_ARG_ENABLE(release_x86,
	AC_HELP_STRING([--enable-release_x86],
		[Use 'RELEASE_X86' Configuration [default=YES]]),
		enable_release_x86=yes, enable_release_x86=no)
AM_CONDITIONAL(ENABLE_RELEASE_X86, test x$enable_release_x86 = xyes)
if test "x$enable_release_x86" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_RELEASE_X86, true)
	enable_release_x86=yes
fi

AC_ARG_ENABLE(appindicator,
	AC_HELP_STRING([--enable-appindicator],
		[Add Ubuntu Application Indicator support [default=NO]]),
		appindicator=yes, appindicator=no)
AM_CONDITIONAL([APPINDICATOR], test x$appindicator = xyes)


dnl package checks, common for all configs
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([GCONF_SHARP_20], [gconf-sharp-2.0])
PKG_CHECK_MODULES([GNOME_SHARP_20], [gnome-sharp-2.0])

AS_IF([test x$appindicator = xyes], [PKG_CHECK_MODULES([APPINDICATOR_SHARP_01], [appindicator-sharp-0.1])])

dnl package checks, per config

# Define Getext
GETTEXT_PACKAGE=glippy
AC_SUBST(GETTEXT_PACKAGE)
AC_DEFINE_UNQUOTED([GETTEXT_PACKAGE],["$GETTEXT_PACKAGE"], [Gettext package])

AM_GNU_GETTEXT([external])
AM_GNU_GETTEXT_VERSION(0.16.1)

AC_CONFIG_FILES([
src/xhotkeys/Makefile
src/core/Makefile
src/upload/Makefile
src/urlshortener/Makefile
src/snippets/Makefile
src/actions/Makefile
src/indicator/Makefile
src/screenshot/Makefile
po/Makefile
src/application/Makefile
src/tray/Makefile
src/exec/glippy
src/exec/Makefile
Makefile

])

AC_OUTPUT
