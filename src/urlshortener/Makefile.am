
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG_X86
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = ../../build/Debug/libglippyurlshortener.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../build/Debug/libglippycore.dll
BUILD_DIR = ../../build/Debug

LIBGLIPPYURLSHORTENER_DLL_MDB_SOURCE=../../build/Debug/libglippyurlshortener.dll.mdb
LIBGLIPPYURLSHORTENER_DLL_MDB=$(BUILD_DIR)/libglippyurlshortener.dll.mdb
LIBGLIPPYCORE_DLL_SOURCE=../../build/Debug/libglippycore.dll

endif

if ENABLE_RELEASE_X86
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = ../../build/Release/libglippyurlshortener.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../../build/Release/libglippycore.dll
BUILD_DIR = ../../build/Release

LIBGLIPPYURLSHORTENER_DLL_MDB=
LIBGLIPPYCORE_DLL_SOURCE=../../build/Release/libglippycore.dll

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(LIBGLIPPYURLSHORTENER_DLL_MDB) \
	$(LIBGLIPPYCORE_DLL)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) 

FILES = \
	gtk-gui/generated.cs \
	AssemblyInfo.cs \
	UrlShortener.cs \
	Shorteners.cs \
	gtk-gui/Glippy.UrlShortener.UrlShortenerPreferencesPage.cs \
	UrlShortenerPreferencesPage.cs \
	SettingsKeys.cs \
	ShortenerService.cs

DATA_FILES = 

RESOURCES = \
	gtk-gui/gui.stetic 

EXTRAS = 

REFERENCES =  \
	System \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	System.Xml \
	System.Core

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

include $(top_srcdir)/Makefile.include

LIBGLIPPYCORE_DLL = $(BUILD_DIR)/libglippycore.dll



$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
