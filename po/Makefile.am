
EXTRA_DIST =  


# Warning: This is an automatically generated file, do not edit!

CLEANFILES = $(GMO_FILES)

include $(top_srcdir)/Makefile.include

PACKAGE = @PACKAGE@
BUILD_DIR = ../build/locale
INSTALL_DIR = $(DESTDIR)@expanded_datadir@/locale

FILES = \
	pl.po \
	de.po \
	es.po \
	it.po \
	ja.po \
	fr.po \
	id.po \
	ru.po

GMO_FILES = $(patsubst %.po,$(BUILD_DIR)/%.gmo,$(FILES))
MO_FILES = $(foreach po,$(FILES), $(INSTALL_DIR)/$(basename $(po))/LC_MESSAGES/$(PACKAGE).mo)

all: $(GMO_FILES)

install: $(MO_FILES)

uninstall:
	for lang in $(basename $(FILES)); do \
		rm -f $(INSTALL_DIR)/$$lang/LC_MESSAGES/$(PACKAGE).mo; \
	done

$(GMO_FILES): $(BUILD_DIR)/%.gmo: %.po
	mkdir -p $(BUILD_DIR)
	msgfmt '$<' -o '$@'
	
$(MO_FILES): $(INSTALL_DIR)/%/LC_MESSAGES/$(PACKAGE).mo : $(BUILD_DIR)/%.gmo
	mkdir -p $(dir $@)
	@INSTALL_DATA@ '$<' '$@'
