EXTERNALS=externals
LIBWEBSOCKETS=libwebsockets
ARDUINOJSON=ArduinoJson
GTEST=gtest-1.7.0
DEPS=deps
LIB=lib
TEST=test
INC=include
LIBOUT=libout
OUT=out
APP=app
CPP=g++
LIBCFLAGS=-I$(DEPS)/$(INC) -Wall -fPIC
LIBDEPLIBS=-L$(DEPS)/$(LIB) -lArduinoJson -lssl -lcrypto -lwebsockets -lcurl
APPCFLAGS=-I$(LIBOUT)/$(INC) -Wall
APPDEPLIBS=-L$(LIBOUT) -lthriotclient
TESTCFLAGS=-std=gnu++0x -I$(LIBOUT)/$(INC) -I$(DEPS)/$(TEST)/$(INC) -Wall
TESTDEPLIBS=-L$(LIBOUT) -lthriotclient -L$(DEPS)/$(TEST)/$(LIB) -lgtest -lgtest_main -lpthread
LIBNAME=libthriotclient.so

_LIBOBJS = CurlRestConnection.o LibWebSocketConnection.o base64.o ClientSettings.o \
	Management/ManagementClient.o Management/common.o \
	Management/UserManagementClient.o Management/TelemetryDataSinksMetadataClient.o \
	Management/CompanyManagementClient.o Management/ServiceManagementClient.o \
	Management/NetworkManagementClient.o Management/DeviceManagementClient.o \
	Platform/OccasionallyConnectionClient.o Platform/PersistentConnectionClient.o \
	Platform/PersistentConnectionInternalClient.o \
	Reporting/ReportingClient.o Reporting/DeviceClient.o Reporting/NetworkClient.o

LIBOBJS = $(patsubst %,$(LIB)/obj/%, $(_LIBOBJS))

_TESTOBJS = usertest.o companytest.o servicetest.o networktest.o devicetest.o telemetrymetadatatest.o \
		occasionalcctest.o persistentcctest.o reportingdevctest.o reportingnetctest.o \
		common.o TestEnvironment.o

TESTOBJS = $(patsubst %,$(TEST)/obj/%, $(_TESTOBJS))

_APPOBJS = app.o 
APPOBJS = $(patsubst %,$(APP)/obj/%, $(_APPOBJS))

all: preparedeps lib test app

lib: preparelib buildlib

test: preparetest buildtest 

app: prepareapp buildapp 

preparelib:
	mkdir -p $(LIB)/obj
	mkdir -p $(LIB)/obj/Management
	mkdir -p $(LIB)/obj/Platform
	mkdir -p $(LIB)/obj/Reporting

buildlib: $(LIBOBJS)
	mkdir -p $(LIBOUT)
	$(CPP) -shared -Wl,--no-undefined,-soname,$(LIBNAME) -o $(LIBOUT)/$(LIBNAME) $^ $(LIBDEPLIBS)
	mkdir -p $(LIBOUT)/include
	cp $(LIB)/Management/ManagementClient.h $(LIBOUT)/include
	cp $(LIB)/Platform/PlatformClient.h $(LIBOUT)/include
	cp $(LIB)/Reporting/ReportingClient.h $(LIBOUT)/include
	cp $(LIB)/ClientSettings.h $(LIBOUT)/include
	cp $(LIBOUT)/$(LIBNAME) $(OUT)/$(LIBNAME)

$(LIB)/obj/%.o: $(LIB)/%.cpp $(LIB)/*.h $(DEPS)/$(INC)/*.h
	$(CPP) $< -c -o $@ $(LIBCFLAGS)

$(LIB)/obj/Management/%.o: $(LIB)/Management/%.cpp $(LIB)/Management/*.h $(LIB)/*.h $(DEPS)/$(INC)/*.h
	$(CPP) $< -c -o $@ $(LIBCFLAGS)

$(LIB)/obj/Platform/%.o: $(LIB)/Platform/%.cpp $(LIB)/Platform/*.h $(LIB)/*.h $(DEPS)/$(INC)/*.h
	$(CPP) $< -c -o $@ $(LIBCFLAGS)

$(LIB)/obj/Reporting/%.o: $(LIB)/Reporting/%.cpp $(LIB)/Reporting/*.h $(LIB)/*.h $(DEPS)/$(INC)/*.h
	$(CPP) $< -c -o $@ $(LIBCFLAGS)

preparetest:
	mkdir -p $(TEST)/obj

buildtest: $(TESTOBJS) 
	mkdir -p $(OUT)
	$(CPP) -Wl,-R,'$$ORIGIN',-rpath=$(OUT) -o $(OUT)/test $^ $(TESTDEPLIBS) 

$(TEST)/obj/%.o: $(TEST)/%.cpp $(TEST)/*.h $(LIBOUT)/$(INC)/*.h $(DEPS)/$(TEST)/$(INC)
	$(CPP) $< -c -o $@ $(TESTCFLAGS)


prepareapp:
	mkdir -p $(APP)/obj

buildapp: $(APPOBJS) 
	mkdir -p $(OUT)
	$(CPP) -Wl,-R,'$$ORIGIN',-rpath=$(OUT) -o $(OUT)/app $^ $(APPDEPLIBS) 

$(APP)/obj/%.o: $(APP)/%.cpp $(LIBOUT)/$(INC)/*.h
	$(CPP) $< -c -o $@ $(APPCFLAGS)


preparedeps: libwebsockets arduinojson gtest copydeps

clean: cleandeps cleanlibout cleanlib cleanout cleanapp cleantest

cleandeps:
	rm -R -f $(EXTERNALS)
	rm -R -f $(DEPS)

cleanlibout:
	rm -R -f $(LIBOUT)

cleanout:
	rm -R -f $(OUT)

cleanlib:
	rm -R -f $(LIB)/obj
	rm -R -f $(LIBOUT)/include
	rm -R -f $(LIBOUT)/*
	rm -f $(OUT)/$(LIBNAME)

cleantest:
	rm -R -f $(TEST)/obj
	rm -f $(OUT)/app

cleanapp:
	rm -R -f $(APP)/obj
	rm -f $(OUT)/app

libwebsockets:
	mkdir -p $(EXTERNALS)
	cd $(EXTERNALS) && \
	rm -R -f $(LIBWEBSOCKETS) && \
	git clone https://github.com/warmcat/libwebsockets.git && \
 	cd $(LIBWEBSOCKETS) && \
	mkdir build && \
	cd build && \
	cmake .. && \
	make

arduinojson:
	mkdir -p $(EXTERNALS)
	cd $(EXTERNALS) && \
	rm -R -f $(ARDUINOJSON) && \
	git clone https://github.com/bblanchon/ArduinoJson.git && \
	cd $(ARDUINOJSON) && \
	sed -i 's/-fno-exceptions/-fno-exceptions\n\t\t-fPIC/' src/CMakeLists.txt && \
	cmake . && \
	make

gtest:
	mkdir -p $(EXTERNALS)
	cd $(EXTERNALS) && \
	rm -R -f $(GTEST) && \
	wget http://googletest.googlecode.com/files/gtest-1.7.0.zip && \
	unzip gtest-1.7.0.zip && \
	cd $(GTEST) && \
	cmake . && \
	make

copydeps:
	mkdir -p $(OUT)
	mkdir -p $(DEPS)/$(LIB)
	mkdir -p $(DEPS)/$(INC)
	mkdir -p $(DEPS)/$(TEST)/$(LIB)
	mkdir -p $(DEPS)/$(TEST)/$(INC)
	cp $(EXTERNALS)/$(LIBWEBSOCKETS)/lib/libwebsockets.h $(DEPS)/$(INC)
	cp $(EXTERNALS)/$(LIBWEBSOCKETS)/build/lws_config.h $(DEPS)/$(INC)
	cp $(EXTERNALS)/$(LIBWEBSOCKETS)/build/lib/libwebsockets.so $(DEPS)/$(LIB)
	cp $(EXTERNALS)/$(LIBWEBSOCKETS)/build/lib/libwebsockets.so $(OUT)
	ln -f -s libwebsockets.so $(OUT)/libwebsockets.so.6.0.0
	ln -f -s libwebsockets.so $(OUT)/libwebsockets.so.6
	
	cp -R $(EXTERNALS)/$(ARDUINOJSON)/include/* $(DEPS)/$(INC)
	cp $(EXTERNALS)/$(ARDUINOJSON)/lib/libArduinoJson.a $(DEPS)/$(LIB)
	
	cp -R $(EXTERNALS)/$(GTEST)/include/* $(DEPS)/$(TEST)/$(INC)
	cp $(EXTERNALS)/$(GTEST)/libgtest.a $(DEPS)/$(TEST)/$(LIB)
	cp $(EXTERNALS)/$(GTEST)/libgtest_main.a $(DEPS)/$(TEST)/$(LIB)
	
