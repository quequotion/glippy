icons16dir = $(datadir)/icons/hicolor/16x16/apps
icons16_DATA = \
	data/icons/16/glippy-panel.svg \
	data/icons/16/glippy.svg              

icons22dir = $(datadir)/icons/hicolor/22x22/apps
icons22_DATA = \
	data/icons/22/glippy-panel.svg \
	data/icons/22/glippy.svg              

icons24dir = $(datadir)/icons/hicolor/24x24/apps
icons24_DATA = \
	data/icons/24/glippy-panel.svg \
	data/icons/24/glippy.svg              

icons48dir = $(datadir)/icons/hicolor/48x48/apps
icons48_DATA = \
	data/icons/48/glippy-panel.svg \
	data/icons/48/glippy.svg         
	
iconsdir = $(datadir)/icons/hicolor/scalable/apps
icons_DATA = \
	data/icons/scalable/glippy-panel.svg \
	data/icons/scalable/glippy.svg              
     
desktopdir = $(datadir)/applications
desktop_DATA = data/glippy.desktop

manndir = ${datadir}/man/man1
mann_DATA = data/glippy.1.gz

schemadir   = @GCONF_SCHEMA_FILE_DIR@
schema_DATA = data/glippy.schemas


EXTRA_DIST = autogen.sh $(icons16_DATA) $(icons22_DATA) $(icons24_DATA) $(icons48_DATA) $(icons_DATA) ${desktop_DATA} ${man_DATA} expansions.m4
  

#Warning: This is an automatically generated file, do not edit!
SUBDIRS =  src/xhotkeys src/core src/upload src/urlshortener src/snippets src/actions src/screenshot po src/application src/tray

if APPINDICATOR
SUBDIRS += src/indicator
endif

SUBDIRS += src/exec

install-data-local:
	GCONF_CONFIG_SOURCE=$(GCONF_SCHEMA_CONFIG_SOURCE) gconftool-2 --makefile-install-rule $(srcdir)/$(schema_DATA)

