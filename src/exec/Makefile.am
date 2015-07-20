
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG_X86
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG" "-main:Glippy.Program"
ASSEMBLY = ../../build/Debug/glippy.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../../build/Debug/libglippycore.dll \
	../../build/Debug/libglippy.dll
BUILD_DIR = ../../build/Debug

GLIPPY_EXE_MDB_SOURCE=../../build/Debug/glippy.exe.mdb
GLIPPY_EXE_MDB=$(BUILD_DIR)/glippy.exe.mdb
LIBGLIPPYCORE_DLL_SOURCE=../../build/Debug/libglippycore.dll
LIBGLIPPY_DLL_SOURCE=../../build/Debug/libglippy.dll
LIBGLIPPYXHOTKEYS_DLL_SOURCE=../../build/Debug/libglippyxhotkeys.dll

LIBGLIPPYURLSHORTENER_DLL_SOURCE=../../build/Debug/libglippyurlshortener.dll
LIBGLIPPYUPLOAD_DLL_SOURCE=../../build/Debug/libglippyupload.dll
LIBGLIPPYTRAY_DLL_SOURCE=../../build/Debug/libglippytray.dll
LIBGLIPPYSNIPPETS_DLL_SOURCE=../../build/Debug/libglippysnippets.dll
LIBGLIPPYSCREENSHOT_DLL_SOURCE=../../build/Debug/libglippyscreenshot.dll
LIBGLIPPYACTIONS_DLL_SOURCE=../../build/Debug/libglippyactions.dll

if APPINDICATOR
LIBGLIPPYINDICATOR_DLL_SOURCE=../../build/Debug/libglippyindicator.dll
endif

endif

if ENABLE_RELEASE_X86
ASSEMBLY_COMPILER_COMMAND = dmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ "-main:Glippy.Program"
ASSEMBLY = ../../build/Release/glippy.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES =  \
	../../build/Release/libglippycore.dll \
	../../build/Release/libglippy.dll
BUILD_DIR = ../../build/Release

GLIPPY_EXE_MDB=
LIBGLIPPYCORE_DLL_SOURCE=../../build/Release/libglippycore.dll
LIBGLIPPY_DLL_SOURCE=../../build/Release/libglippy.dll
LIBGLIPPYXHOTKEYS_DLL_SOURCE=../../build/Release/libglippyxhotkeys.dll

LIBGLIPPYURLSHORTENER_DLL_SOURCE=../../build/Release/libglippyurlshortener.dll
LIBGLIPPYUPLOAD_DLL_SOURCE=../../build/Release/libglippyupload.dll
LIBGLIPPYTRAY_DLL_SOURCE=../../build/Release/libglippytray.dll
LIBGLIPPYSNIPPETS_DLL_SOURCE=../../build/Release/libglippysnippets.dll
LIBGLIPPYSCREENSHOT_DLL_SOURCE=../../build/Release/libglippyscreenshot.dll
LIBGLIPPYACTIONS_DLL_SOURCE=../../build/Release/libglippyactions.dll

if APPINDICATOR
LIBGLIPPYINDICATOR_DLL_SOURCE=../../build/Debug/libglippyindicator.dll
endif

endif

AL=al
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GLIPPY_EXE) \
	$(LIBGLIPPYCORE_DLL) \
	$(LIBGLIPPY_DLL) \
	$(LIBGLIPPYXHOTKEYS_DLL) \
	$(LIBGLIPPYURLSHORTENER_DLL) \
	$(LIBGLIPPYUPLOAD_DLL) \
	$(LIBGLIPPYTRAY_DLL) \
	$(LIBGLIPPYSNIPPETS_DLL) \
	$(LIBGLIPPYSCREENSHOT_DLL) \
	$(LIBGLIPPYACTIONS_DLL)

if APPINDICATOR
PROGRAMFILES += $(LIBGLIPPYINDICATOR_DLL)
endif



BINARIES = \
	$(GLIPPY)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	Program.cs \
	AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	glippy.in 

REFERENCES =  \
	System \
	$(GNOME_SHARP_20_LIBS) \
	Mono.Posix \
	System.Core

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

GLIPPY_EXE = $(BUILD_DIR)/glippy.exe
LIBGLIPPYCORE_DLL = $(BUILD_DIR)/libglippycore.dll
LIBGLIPPY_DLL = $(BUILD_DIR)/libglippy.dll
LIBGLIPPYXHOTKEYS_DLL = $(BUILD_DIR)/libglippyxhotkeys.dll

LIBGLIPPYURLSHORTENER_DLL=$(BUILD_DIR)/libglippyurlshortener.dll
LIBGLIPPYUPLOAD_DLL=$(BUILD_DIR)/libglippyupload.dll
LIBGLIPPYTRAY_DLL=$(BUILD_DIR)/libglippytray.dll
LIBGLIPPYSNIPPETS_DLL=$(BUILD_DIR)/libglippysnippets.dll
LIBGLIPPYSCREENSHOT_DLL=$(BUILD_DIR)/libglippyscreenshot.dll
LIBGLIPPYACTIONS_DLL=$(BUILD_DIR)/libglippyactions.dll

if APPINDICATOR
LIBGLIPPYINDICATOR_DLL=$(BUILD_DIR)/libglippyindicator.dll
endif

GLIPPY = $(BUILD_DIR)/glippy

$(eval $(call emit-deploy-wrapper,GLIPPY,glippy,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
