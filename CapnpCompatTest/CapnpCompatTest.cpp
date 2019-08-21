// Build preconditions:
// - Have vcpkg installed
// > vcpkg integrate install
// > vcpkg install capnproto

#include <iostream>
#include <thread>
#include <capnp/ez-rpc.h>
#include <kj/common.h>
#include "test.capnp.h"

using namespace std;
using namespace capnp;
using namespace kj;
using namespace capnproto_test::capnp::test;

void PrintUsage(const char* argv[])
{
	cerr << "usage: " << argv[0] << "{client|server} HOST[:PORT]" << std::endl;
}

inline Data::Reader data(const char* str) {
	return Data::Reader(reinterpret_cast<const byte*>(str), strlen(str));
}

template <typename Builder>
void genericInitTestMessage(Builder builder) {
	builder.setVoidField(VOID);
	builder.setVoidField();  // Means the same as above.
	builder.setBoolField(true);
	builder.setInt8Field(-123);
	builder.setInt16Field(-12345);
	builder.setInt32Field(-12345678);
	builder.setInt64Field(-123456789012345ll);
	builder.setUInt8Field(234u);
	builder.setUInt16Field(45678u);
	builder.setUInt32Field(3456789012u);
	builder.setUInt64Field(12345678901234567890ull);
	builder.setFloat32Field(1234.5);
	builder.setFloat64Field(-123e45);
	builder.setTextField("foo");
	builder.setDataField(data("bar"));
	{
		auto subBuilder = builder.initStructField();
		subBuilder.setVoidField(VOID);
		subBuilder.setBoolField(true);
		subBuilder.setInt8Field(-12);
		subBuilder.setInt16Field(3456);
		subBuilder.setInt32Field(-78901234);
		subBuilder.setInt64Field(56789012345678ll);
		subBuilder.setUInt8Field(90u);
		subBuilder.setUInt16Field(1234u);
		subBuilder.setUInt32Field(56789012u);
		subBuilder.setUInt64Field(345678901234567890ull);
		subBuilder.setFloat32Field(-1.25e-10f);
		subBuilder.setFloat64Field(345);
		subBuilder.setTextField("baz");
		subBuilder.setDataField(data("qux"));
		{
			auto subSubBuilder = subBuilder.initStructField();
			subSubBuilder.setTextField("nested");
			subSubBuilder.initStructField().setTextField("really nested");
		}
		subBuilder.setEnumField(TestEnum::BAZ);

		subBuilder.setVoidList({ VOID, VOID, VOID });
		subBuilder.setBoolList({ false, true, false, true, true });
		subBuilder.setInt8List({ 12, -34, -0x80, 0x7f });
		subBuilder.setInt16List({ 1234, -5678, -0x8000, 0x7fff });
		// gcc warns on -0x800... and the only work-around I could find was to do -0x7ff...-1.
		subBuilder.setInt32List({ 12345678, -90123456, -0x7fffffff - 1, 0x7fffffff });
		subBuilder.setInt64List({ 123456789012345ll, -678901234567890ll, -0x7fffffffffffffffll - 1, 0x7fffffffffffffffll });
		subBuilder.setUInt8List({ 12u, 34u, 0u, 0xffu });
		subBuilder.setUInt16List({ 1234u, 5678u, 0u, 0xffffu });
		subBuilder.setUInt32List({ 12345678u, 90123456u, 0u, 0xffffffffu });
		subBuilder.setUInt64List({ 123456789012345ull, 678901234567890ull, 0ull, 0xffffffffffffffffull });
		subBuilder.setFloat32List({ 0, 1234567, 1e37f, -1e37f, 1e-37f, -1e-37f });
		subBuilder.setFloat64List({ 0, 123456789012345, 1e306, -1e306, 1e-306, -1e-306 });
		subBuilder.setTextList({ "quux", "corge", "grault" });
		subBuilder.setDataList({ data("garply"), data("waldo"), data("fred") });
		{
			auto listBuilder = subBuilder.initStructList(3);
			listBuilder[0].setTextField("x structlist 1");
			listBuilder[1].setTextField("x structlist 2");
			listBuilder[2].setTextField("x structlist 3");
		}
		subBuilder.setEnumList({ TestEnum::QUX, TestEnum::BAR, TestEnum::GRAULT });
	}
	builder.setEnumField(TestEnum::CORGE);

	builder.initVoidList(6);
	builder.setBoolList({ true, false, false, true });
	builder.setInt8List({ 111, -111 });
	builder.setInt16List({ 11111, -11111 });
	builder.setInt32List({ 111111111, -111111111 });
	builder.setInt64List({ 1111111111111111111ll, -1111111111111111111ll });
	builder.setUInt8List({ 111u, 222u });
	builder.setUInt16List({ 33333u, 44444u });
	builder.setUInt32List({ 3333333333u });
	builder.setUInt64List({ 11111111111111111111ull });
	builder.setFloat32List({ 5555.5, kj::inf(), -kj::inf(), kj::nan() });
	builder.setFloat64List({ 7777.75, kj::inf(), -kj::inf(), kj::nan() });
	builder.setTextList({ "plugh", "xyzzy", "thud" });
	builder.setDataList({ data("oops"), data("exhausted"), data("rfc3092") });
	{
		auto listBuilder = builder.initStructList(3);
		listBuilder[0].setTextField("structlist 1");
		listBuilder[1].setTextField("structlist 2");
		listBuilder[2].setTextField("structlist 3");
	}
	builder.setEnumList({ TestEnum::FOO, TestEnum::GARPLY });
}

template<typename T1, typename T2>
void EXPECT_EQ(const T1& expected, const T2& actual)
{
	if (expected != actual)
		cout << "Fail" << endl;
}

void EXPECT_TRUE(bool truth)
{
	if (!truth)
		cout << "Fail: Not true" << endl;
}

void EXPECT_FALSE(bool lie)
{
	if (lie)
		cout << "Fail: Not false" << endl;
}

template<typename T1, typename T2>
void ASSERT_EQ(const T1& expected, const T2& actual)
{
	if (expected != actual)
	{
		cout << "Fatal" << endl;
		throw runtime_error("Failed assertion");
	}
}

void EXPECT_FLOAT_EQ(float expected, float actual)
{
	EXPECT_EQ(expected, actual);
}

void EXPECT_DOUBLE_EQ(double expected, double actual)
{
	EXPECT_EQ(expected, actual);
}

template <typename T>
inline void checkElement(T a, T b) {
	EXPECT_EQ(a, b);
}

template <>
inline void checkElement<float>(float a, float b) {
	EXPECT_FLOAT_EQ(a, b);
}

template <>
inline void checkElement<double>(double a, double b) {
	EXPECT_DOUBLE_EQ(a, b);
}

template <typename T, typename L = typename T::Reads>
void checkList(T reader, std::initializer_list<decltype(reader[0])> expected) {
	ASSERT_EQ(expected.size(), reader.size());
	for (uint i = 0; i < expected.size(); i++) {
		checkElement<decltype(reader[0])>(expected.begin()[i], reader[i]);
	}
}

template <typename T, typename L = typename T::Builds, bool = false>
void checkList(T reader, std::initializer_list<decltype(typename L::Reader()[0])> expected) {
	ASSERT_EQ(expected.size(), reader.size());
	for (uint i = 0; i < expected.size(); i++) {
		checkElement<decltype(typename L::Reader()[0])>(expected.begin()[i], reader[i]);
	}
}

template <typename Reader>
void genericCheckTestMessage(Reader reader) {
	EXPECT_EQ(VOID, reader.getVoidField());
	EXPECT_EQ(true, reader.getBoolField());
	EXPECT_EQ(-123, reader.getInt8Field());
	EXPECT_EQ(-12345, reader.getInt16Field());
	EXPECT_EQ(-12345678, reader.getInt32Field());
	EXPECT_EQ(-123456789012345ll, reader.getInt64Field());
	EXPECT_EQ(234u, reader.getUInt8Field());
	EXPECT_EQ(45678u, reader.getUInt16Field());
	EXPECT_EQ(3456789012u, reader.getUInt32Field());
	EXPECT_EQ(12345678901234567890ull, reader.getUInt64Field());
	EXPECT_FLOAT_EQ(1234.5f, reader.getFloat32Field());
	EXPECT_DOUBLE_EQ(-123e45, reader.getFloat64Field());
	EXPECT_EQ("foo", reader.getTextField());
	EXPECT_EQ(data("bar"), reader.getDataField());
	{
		auto subReader = reader.getStructField();
		EXPECT_EQ(VOID, subReader.getVoidField());
		EXPECT_EQ(true, subReader.getBoolField());
		EXPECT_EQ(-12, subReader.getInt8Field());
		EXPECT_EQ(3456, subReader.getInt16Field());
		EXPECT_EQ(-78901234, subReader.getInt32Field());
		EXPECT_EQ(56789012345678ll, subReader.getInt64Field());
		EXPECT_EQ(90u, subReader.getUInt8Field());
		EXPECT_EQ(1234u, subReader.getUInt16Field());
		EXPECT_EQ(56789012u, subReader.getUInt32Field());
		EXPECT_EQ(345678901234567890ull, subReader.getUInt64Field());
		EXPECT_FLOAT_EQ(-1.25e-10f, subReader.getFloat32Field());
		EXPECT_DOUBLE_EQ(345, subReader.getFloat64Field());
		EXPECT_EQ("baz", subReader.getTextField());
		EXPECT_EQ(data("qux"), subReader.getDataField());
		{
			auto subSubReader = subReader.getStructField();
			EXPECT_EQ("nested", subSubReader.getTextField());
			EXPECT_EQ("really nested", subSubReader.getStructField().getTextField());
		}
		EXPECT_EQ(TestEnum::BAZ, subReader.getEnumField());

		checkList(subReader.getVoidList(), { VOID, VOID, VOID });
		checkList(subReader.getBoolList(), { false, true, false, true, true });
		checkList(subReader.getInt8List(), { 12, -34, -0x80, 0x7f });
		checkList(subReader.getInt16List(), { 1234, -5678, -0x8000, 0x7fff });
		// gcc warns on -0x800... and the only work-around I could find was to do -0x7ff...-1.
		checkList(subReader.getInt32List(), { 12345678, -90123456, -0x7fffffff - 1, 0x7fffffff });
		checkList(subReader.getInt64List(), { 123456789012345ll, -678901234567890ll, -0x7fffffffffffffffll - 1, 0x7fffffffffffffffll });
		checkList(subReader.getUInt8List(), { 12u, 34u, 0u, 0xffu });
		checkList(subReader.getUInt16List(), { 1234u, 5678u, 0u, 0xffffu });
		checkList(subReader.getUInt32List(), { 12345678u, 90123456u, 0u, 0xffffffffu });
		checkList(subReader.getUInt64List(), { 123456789012345ull, 678901234567890ull, 0ull, 0xffffffffffffffffull });
		checkList(subReader.getFloat32List(), { 0.0f, 1234567.0f, 1e37f, -1e37f, 1e-37f, -1e-37f });
		checkList(subReader.getFloat64List(), { 0.0, 123456789012345.0, 1e306, -1e306, 1e-306, -1e-306 });
		checkList(subReader.getTextList(), { "quux", "corge", "grault" });
		checkList(subReader.getDataList(), { data("garply"), data("waldo"), data("fred") });
		{
			auto listReader = subReader.getStructList();
			ASSERT_EQ(3u, listReader.size());
			EXPECT_EQ("x structlist 1", listReader[0].getTextField());
			EXPECT_EQ("x structlist 2", listReader[1].getTextField());
			EXPECT_EQ("x structlist 3", listReader[2].getTextField());
		}
		checkList(subReader.getEnumList(), { TestEnum::QUX, TestEnum::BAR, TestEnum::GRAULT });
	}
	EXPECT_EQ(TestEnum::CORGE, reader.getEnumField());

	EXPECT_EQ(6u, reader.getVoidList().size());
	checkList(reader.getBoolList(), { true, false, false, true });
	checkList(reader.getInt8List(), { 111, -111 });
	checkList(reader.getInt16List(), { 11111, -11111 });
	checkList(reader.getInt32List(), { 111111111, -111111111 });
	checkList(reader.getInt64List(), { 1111111111111111111ll, -1111111111111111111ll });
	checkList(reader.getUInt8List(), { 111u, 222u });
	checkList(reader.getUInt16List(), { 33333u, 44444u });
	checkList(reader.getUInt32List(), { 3333333333u });
	checkList(reader.getUInt64List(), { 11111111111111111111ull });
	{
		auto listReader = reader.getFloat32List();
		ASSERT_EQ(4u, listReader.size());
		EXPECT_EQ(5555.5f, listReader[0]);
		EXPECT_EQ(kj::inf(), listReader[1]);
		EXPECT_EQ(-kj::inf(), listReader[2]);
		EXPECT_TRUE(isNaN(listReader[3]));
	}
	{
		auto listReader = reader.getFloat64List();
		ASSERT_EQ(4u, listReader.size());
		EXPECT_EQ(7777.75, listReader[0]);
		EXPECT_EQ(kj::inf(), listReader[1]);
		EXPECT_EQ(-kj::inf(), listReader[2]);
		EXPECT_TRUE(isNaN(listReader[3]));
	}
	checkList(reader.getTextList(), { "plugh", "xyzzy", "thud" });
	checkList(reader.getDataList(), { data("oops"), data("exhausted"), data("rfc3092") });
	{
		auto listReader = reader.getStructList();
		ASSERT_EQ(3u, listReader.size());
		EXPECT_EQ("structlist 1", listReader[0].getTextField());
		EXPECT_EQ("structlist 2", listReader[1].getTextField());
		EXPECT_EQ("structlist 3", listReader[2].getTextField());
	}
	checkList(reader.getEnumList(), { TestEnum::FOO, TestEnum::GARPLY });
}

class TestInterfaceImpl final : public TestInterface::Server
{
	int& callCount_;

public:
	TestInterfaceImpl(int& callCount):
		callCount_(callCount)
	{
	}

	~TestInterfaceImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> foo(FooContext context) override
	{
		++callCount_;
		auto params = context.getParams();
		cout << "foo " << params.getI() << " " << params.getJ() << endl;
		context.initResults().setX("foo");
		return kj::READY_NOW;
	}

	kj::Promise<void> baz(BazContext context) override
	{
		++callCount_;
		cout << "baz" << endl;
		try
		{
			genericCheckTestMessage(context.getParams().getS());
		}
		catch (const runtime_error&)
		{
		}
		cout << "baz fin" << endl;
		return kj::READY_NOW;
	}

};

class TestCapDestructor final : public TestInterface::Server {
	// Implementation of TestInterface that notifies when it is destroyed.

public:
	TestCapDestructor(kj::Own<kj::PromiseFulfiller<void>>&& fulfiller)
		: fulfiller(kj::mv(fulfiller)), impl(dummy) {}

	~TestCapDestructor() {
		fulfiller->fulfill();
	}

	kj::Promise<void> foo(FooContext context) {
		return impl.foo(context);
	}

private:
	kj::Own<kj::PromiseFulfiller<void>> fulfiller;
	int dummy = 0;
	TestInterfaceImpl impl;
};

class TestExtendsImpl final : public TestExtends::Server
{
public:
	~TestExtendsImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> foo(FooContext context) {
		cout << "foo" << endl;
		auto params = context.getParams();
		auto result = context.getResults();
		EXPECT_EQ(321, params.getI());
		EXPECT_FALSE(params.getJ());
		result.setX("bar");
		return kj::READY_NOW;
	}

	kj::Promise<void> grault(GraultContext context) {
		cout << "grault" << endl;
		context.releaseParams();

		genericInitTestMessage(context.getResults());

		return kj::READY_NOW;
	}

};

class TestPipelineImpl final : public TestPipeline::Server
{
public:
	~TestPipelineImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> getCap(GetCapContext context) override {
		cout << "getCap" << endl;

		auto params = context.getParams();
		EXPECT_EQ(234, params.getN());

		auto cap = params.getInCap();
		context.releaseParams();

		auto request = cap.fooRequest();
		request.setI(123);
		request.setJ(true);

		return request.send().then(
			[this, KJ_CPCAP(context)](Response<TestInterface::FooResults>&& response) mutable {
			EXPECT_EQ("foo", response.getX());

			auto result = context.getResults();
			result.setS("bar");
			result.initOutBox().setCap(kj::heap<TestExtendsImpl>());
			cout << "getCap fin" << endl;
		});
	}

	kj::Promise<void> getAnyCap(GetAnyCapContext context) override {
		cout << "getAnyCap" << endl;

		auto params = context.getParams();
		EXPECT_EQ(234, params.getN());

		auto cap = params.getInCap();
		context.releaseParams();

		auto request = cap.castAs<TestInterface>().fooRequest();
		request.setI(123);
		request.setJ(true);

		return request.send().then(
			[this, KJ_CPCAP(context)](Response<TestInterface::FooResults>&& response) mutable {
			EXPECT_EQ("foo", response.getX());

			auto result = context.getResults();
			result.setS("bar");
			result.initOutBox().setCap(kj::heap<TestExtendsImpl>());
			cout << "getAnyCap fin" << endl;
		});
	}

};

class TestCallOrderImpl final : public TestCallOrder::Server
{
public:
	uint32_t count;

	TestCallOrderImpl() : count(0)
	{
	}

	~TestCallOrderImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> getCallSequence(GetCallSequenceContext context) override
	{
		auto result = context.getResults();
		result.setN(count++);
		return kj::READY_NOW;
	}
};

class TestTailCallerImpl final : public TestTailCaller::Server
{
public:
	~TestTailCallerImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> foo(FooContext context) override
	{
		cout << "foo" << endl;

		auto params = context.getParams();
		auto tailRequest = params.getCallee().fooRequest();
		tailRequest.setI(params.getI());
		tailRequest.setT("from TestTailCaller");
		return context.tailCall(kj::mv(tailRequest));
	}
};

class TestTailCalleeImpl final : public TestTailCallee::Server
{
	int& callCount_;

public:
	TestTailCalleeImpl(int& callCount) : callCount_(callCount)
	{
	}

	~TestTailCalleeImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> foo(FooContext context) override
	{
		++callCount_;

		cout << "foo" << endl;

		auto params = context.getParams();
		auto results = context.getResults();

		results.setI(params.getI());
		results.setT(params.getT());
		results.setC(kj::heap<TestCallOrderImpl>());

		return kj::READY_NOW;
	}
};

class HandleImpl final : public TestHandle::Server 
{
public:
	HandleImpl()
	{
		cout << "++" << endl;
	}

	~HandleImpl() 
	{
		cout << "--" << endl;
	}
};

class TestMoreStuffImpl final : public TestMoreStuff::Server
{
public:
	uint32_t callCount;
	TestInterface::Client clientToHold;

	TestMoreStuffImpl() : 
		callCount(0),
		clientToHold(nullptr)
	{
	}

	~TestMoreStuffImpl()
	{
		cout << "~" << endl;
	}

	kj::Promise<void> getCallSequence(GetCallSequenceContext context) override 
	{
		cout << "getCallSequence" << endl;

		auto result = context.getResults();
		result.setN(callCount++);
		return kj::READY_NOW;
	}

	kj::Promise<void> callFoo(CallFooContext context) override 
	{
		++callCount;
		cout << "callFoo" << endl;

		auto params = context.getParams();
		auto cap = params.getCap();

		auto request = cap.fooRequest();
		request.setI(123);
		request.setJ(true);

		return request.send().then(
			[KJ_CPCAP(context)](Response<TestInterface::FooResults>&& response) mutable {
			EXPECT_EQ("foo", response.getX());
			context.getResults().setS("bar");
			cout << "fin" << endl;
		});
	}

	kj::Promise<void> callFooWhenResolved(CallFooWhenResolvedContext context) override {
		++callCount;
		cout << "callFooWhenResolved" << endl;

		auto params = context.getParams();
		auto cap = params.getCap();

		return cap.whenResolved().then([KJ_CPCAP(cap), KJ_CPCAP(context)]() mutable {
			auto request = cap.fooRequest();
			request.setI(123);
			request.setJ(true);

			return request.send().then(
				[KJ_CPCAP(context)](Response<TestInterface::FooResults>&& response) mutable {
				EXPECT_EQ("foo", response.getX());
				context.getResults().setS("bar");
				cout << "fin" << endl;
			});
		});
	}

	kj::Promise<void> neverReturn(NeverReturnContext context) override {
		++callCount;
		cout << "neverReturn" << endl;

		// Attach `cap` to the promise to make sure it is released.
		auto promise = kj::Promise<void>(kj::NEVER_DONE).attach(context.getParams().getCap());

		// Also attach `cap` to the result struct to make sure that is released.
		context.getResults().setCapCopy(context.getParams().getCap());

		context.allowCancellation();
		return kj::mv(promise);
	}

	kj::Promise<void> hold(HoldContext context) override 
	{
		++callCount;
		cout << "hold" << endl;

		auto params = context.getParams();
		clientToHold = params.getCap();
		return kj::READY_NOW;
	}

	kj::Promise<void> callHeld(CallHeldContext context) override 
	{
		++callCount;
		cout << "callHeld" << endl;

		auto request = clientToHold.fooRequest();
		request.setI(123);
		request.setJ(true);

		return request.send().then(
			[KJ_CPCAP(context)](Response<TestInterface::FooResults>&& response) mutable {
			EXPECT_EQ("foo", response.getX());
			context.getResults().setS("bar");
		});
	}

	kj::Promise<void> getHeld(GetHeldContext context) 
	{
		++callCount;
		cout << "getHeld" << endl;

		auto result = context.getResults();
		result.setCap(clientToHold);
		return kj::READY_NOW;
	}

	kj::Promise<void> simpleLoop(int rem)
	{
		if (rem <= 0) {
			return kj::READY_NOW;
		}
		else {
			std::this_thread::sleep_for(std::chrono::milliseconds(1));
			return kj::evalLater([this, rem]() mutable {
				return simpleLoop(rem - 1);
			});
		}
	}

	kj::Promise<void> echo(EchoContext context)
	{
		++callCount;
		cout << "echo" << endl;

		auto params = context.getParams();
		auto result = context.getResults();
		result.setCap(params.getCap());
		return simpleLoop(100); // Loop a little to provoke real promise pipelining
	}

	kj::Promise<void> expectCancel(ExpectCancelContext context) 
	{
		cout << "expectCancel" << endl;
		auto cap = context.getParams().getCap();
		context.allowCancellation();
		// return loop(0, cap, context);
		return kj::NEVER_DONE;
	}

	kj::Promise<void> loop(uint depth, TestInterface::Client cap, ExpectCancelContext context) 
	{
		if (depth > 100000000) {
			cout << "Looped too long, giving up." << endl;
			return kj::READY_NOW;
		}
		else {
			return kj::evalLater([this, depth, KJ_CPCAP(cap), KJ_CPCAP(context)]() mutable {
				return loop(depth + 1, cap, context);
			});
		}
	}

	kj::Promise<void> getHandle(GetHandleContext context) override 
	{
		cout << "getHandle" << endl;
		context.getResults().setHandle(kj::heap<HandleImpl>());
		return kj::READY_NOW;
	}

	kj::Promise<void> getNull(GetNullContext context) 
	{
		cout << "getNull" << endl;
		return kj::READY_NOW;
	}

	kj::Promise<void> getEnormousString(GetEnormousStringContext context) 
	{
		cout << "getEnormousString" << endl;
		context.getResults().initStr(100000000);  // 100MB
		return kj::READY_NOW;
	}
};

void BasicTest(capnp::EzRpcClient& rpc)
{
	cout << "Basic test start" << endl;

	auto client = rpc.getMain<TestInterface>();

	auto request1 = client.fooRequest();
	request1.setI(123);
	request1.setJ(true);
	auto promise1 = request1.send();

	// We used to call bar() after baz(), hence the numbering, but this masked the case where the
	// RPC system actually disconnected on bar() (thus returning an exception, which we decided
	// was expected).
	bool barFailed = false;
	auto request3 = client.barRequest();
	auto promise3 = request3.send().then(
		[](Response<TestInterface::BarResults>&& response) {
			cout << "Expected bar() call to fail." << endl;
		}, [&](kj::Exception&& e) {
			barFailed = true;
		});

	auto request2 = client.bazRequest();
	genericInitTestMessage(request2.initS());
	auto promise2 = request2.send();

	auto response1 = promise1.wait(rpc.getWaitScope());

	EXPECT_EQ("foo", response1.getX());

	auto response2 = promise2.wait(rpc.getWaitScope());

	promise3.wait(rpc.getWaitScope());

	EXPECT_TRUE(barFailed);

	cout << "Basic test end" << endl;
}

void PipeliningTest(capnp::EzRpcClient& rpc)
{
	cout << "Pipelining test start" << endl;

	auto client = rpc.getMain<TestPipeline>();

	int chainedCallCount = 0;

	auto request = client.getCapRequest();
	request.setN(234);
	request.setInCap(kj::heap<TestInterfaceImpl>(chainedCallCount));

	auto promise = request.send();

	auto pipelineRequest = promise.getOutBox().getCap().fooRequest();
	pipelineRequest.setI(321);
	auto pipelinePromise = pipelineRequest.send();

	auto pipelineRequest2 = promise.getOutBox().getCap().castAs<TestExtends>().graultRequest();
	auto pipelinePromise2 = pipelineRequest2.send();

	promise = nullptr;  // Just to be annoying, drop the original promise.

	EXPECT_EQ(0, chainedCallCount);

	auto response = pipelinePromise.wait(rpc.getWaitScope());
	EXPECT_EQ("bar", response.getX());

	auto response2 = pipelinePromise2.wait(rpc.getWaitScope());
	genericCheckTestMessage((TestAllTypes::Reader)response2);

	EXPECT_EQ(1, chainedCallCount);

	cout << "Pipelining test end" << endl;
}

void ReleaseTest(capnp::EzRpcClient& rpc)
{
	cout << "Release test start" << endl;

	auto client = rpc.getMain<TestMoreStuff>();

	auto& waitScope = rpc.getWaitScope();
	auto handle1 = client.getHandleRequest().send().wait(waitScope).getHandle();
	auto promise = client.getHandleRequest().send();
	auto handle2 = promise.wait(waitScope).getHandle();

	string s;
	cout << "sync" << endl;
	cin >> s;

	handle1 = nullptr;
	for (uint i = 0; i < 16; i++) kj::evalLater([]() {}).wait(waitScope);

	cout << "handle1 null" << endl;
	cin >> s;

	handle2 = nullptr;
	for (uint i = 0; i < 16; i++) kj::evalLater([]() {}).wait(waitScope);

	cout << "handle2 null" << endl;
	cin >> s;

	promise = nullptr;
	for (uint i = 0; i < 16; i++) kj::evalLater([]() {}).wait(waitScope);
	waitScope.poll();

	cout << "promise null" << endl;
	cin >> s;

	cout << "Release test end" << endl;
}

void ReleaseOnCancelTest(capnp::EzRpcClient& rpc)
{
	cout << "ReleaseOnCancel test start" << endl;

	auto client = rpc.getMain<TestMoreStuff>();

	auto& waitScope = rpc.getWaitScope();

	{
		auto promise = client.getHandleRequest().send();

		// If the server receives cancellation too early, it won't even return a capability in the
		// results, it will just return "canceled". We want to emulate the case where the return message
		// and the cancel (finish) message cross paths. It turns out that exactly two evalLater()s get
		// us there.
		//
		// TODO(cleanup): This is fragile, but I'm not sure how else to write it without a ton
		//   of scaffolding.
		//kj::evalLater([]() {}).wait(waitScope);
		//kj::evalLater([]() {}).wait(waitScope);
		//waitScope.poll();

		promise.wait(waitScope);
	}

	for (uint i = 0; i < 16; i++) kj::evalLater([]() {}).wait(waitScope);
	waitScope.poll();

	cout << "ReleaseOnCancel test end" << endl;
}

void TailCallTest(capnp::EzRpcClient& rpc)
{
	cout << "TailCall test start" << endl;

	auto caller = rpc.getMain<TestTailCaller>();

	auto& waitScope = rpc.getWaitScope();

	int calleeCallCount = 0;

	TestTailCallee::Client callee(kj::heap<TestTailCalleeImpl>(calleeCallCount));

	auto request = caller.fooRequest();
	request.setI(456);
	request.setCallee(callee);

	auto promise = request.send();

	auto dependentCall0 = promise.getC().getCallSequenceRequest().send();

	auto response = promise.wait(waitScope);
	EXPECT_EQ(456, response.getI());
	EXPECT_EQ("from TestTailCaller", response.getT());

	auto dependentCall1 = promise.getC().getCallSequenceRequest().send();

	auto dependentCall2 = response.getC().getCallSequenceRequest().send();

	EXPECT_EQ(0, dependentCall0.wait(waitScope).getN());
	EXPECT_EQ(1, dependentCall1.wait(waitScope).getN());
	EXPECT_EQ(2, dependentCall2.wait(waitScope).getN());

	EXPECT_EQ(1, calleeCallCount);

	cout << "TailCall test end" << endl;
}

void CancelationTest(capnp::EzRpcClient& rpc)
{
	cout << "Cancelation test start" << endl;

	auto paf = kj::newPromiseAndFulfiller<void>();
	bool destroyed = false;
	auto destructionPromise = paf.promise.then([&]() { destroyed = true; }).eagerlyEvaluate(nullptr);

	auto client = rpc.getMain<TestMoreStuff>();

	kj::Promise<void> promise = nullptr;

	bool returned = false;
	{
		auto request = client.expectCancelRequest();
		request.setCap(kj::heap<TestCapDestructor>(kj::mv(paf.fulfiller)));
		promise = request.send().then(
			[&](Response<TestMoreStuff::ExpectCancelResults>&& response) {
				returned = true;
			}).eagerlyEvaluate(nullptr);
	}

	auto& waitScope = rpc.getWaitScope();
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);

	// We can detect that the method was canceled because it will drop the cap.
	EXPECT_FALSE(destroyed);
	EXPECT_FALSE(returned);

	promise = nullptr;  // request cancellation
	destructionPromise.wait(waitScope);

	EXPECT_TRUE(destroyed);
	EXPECT_FALSE(returned);

	cout << "Cancelation test end" << endl;
}

void PromiseResolveTest(capnp::EzRpcClient& rpc)
{
	cout << "PromiseResolve test start" << endl;

	auto client = rpc.getMain<TestMoreStuff>();

	int chainedCallCount = 0;

	auto request = client.callFooRequest();
	auto request2 = client.callFooWhenResolvedRequest();

	auto paf = kj::newPromiseAndFulfiller<TestInterface::Client>();

	{
		auto fork = paf.promise.fork();
		request.setCap(fork.addBranch());
		request2.setCap(fork.addBranch());
	}

	auto promise = request.send();
	auto promise2 = request2.send();

	auto& waitScope = rpc.getWaitScope();

	// Make sure getCap() has been called on the server side by sending another call and waiting
	// for it.
	EXPECT_EQ(2, client.getCallSequenceRequest().send().wait(waitScope).getN());

	// OK, now fulfill the local promise.
	paf.fulfiller->fulfill(kj::heap<TestInterfaceImpl>(chainedCallCount));

	// We should now be able to wait for getCap() to finish.
	EXPECT_EQ("bar", promise.wait(waitScope).getS());
	EXPECT_EQ("bar", promise2.wait(waitScope).getS());

	EXPECT_EQ(2, chainedCallCount);

	cout << "PromiseResolve test end" << endl;
}

void RetainAndReleaseTest(capnp::EzRpcClient& rpc)
{
	cout << "RetainAndRelease test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto paf = kj::newPromiseAndFulfiller<void>();
	bool destroyed = false;
	auto destructionPromise = paf.promise.then([&]() { destroyed = true; }).eagerlyEvaluate(nullptr);

	{
		auto client = rpc.getMain<TestMoreStuff>();

		{
			auto request = client.holdRequest();
			request.setCap(kj::heap<TestCapDestructor>(kj::mv(paf.fulfiller)));
			request.send().wait(waitScope);
		}

		// Do some other call to add a round trip.
		EXPECT_EQ(1, client.getCallSequenceRequest().send().wait(waitScope).getN());

		// Shouldn't be destroyed because it's being held by the server.
		EXPECT_FALSE(destroyed);

		// We can ask it to call the held capability.
		EXPECT_EQ("bar", client.callHeldRequest().send().wait(waitScope).getS());

		{
			// We can get the cap back from it.
			auto capCopy = client.getHeldRequest().send().wait(waitScope).getCap();

			{
				// And call it, without any network communications.
				auto request = capCopy.fooRequest();
				request.setI(123);
				request.setJ(true);
				EXPECT_EQ("foo", request.send().wait(waitScope).getX());
			}

			{
				// We can send another copy of the same cap to another method, and it works.
				auto request = client.callFooRequest();
				request.setCap(capCopy);
				EXPECT_EQ("bar", request.send().wait(waitScope).getS());
			}
		}

		// Give some time to settle.
		EXPECT_EQ(5, client.getCallSequenceRequest().send().wait(waitScope).getN());
		EXPECT_EQ(6, client.getCallSequenceRequest().send().wait(waitScope).getN());
		EXPECT_EQ(7, client.getCallSequenceRequest().send().wait(waitScope).getN());

		// Can't be destroyed, we haven't released it.
		EXPECT_FALSE(destroyed);

		// Different from original test, request the server to hold a null cap, 
		// leading to the destruction of the former cap.
		auto holdNull = client.holdRequest();
		holdNull.send().wait(waitScope);
	}

	destructionPromise.wait(waitScope);
	EXPECT_TRUE(destroyed);

	cout << "RetainAndRelease test end" << endl;
}

void CancelTest(capnp::EzRpcClient& rpc)
{
	cout << "Cancel test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto client = rpc.getMain<TestMoreStuff>();

	auto paf = kj::newPromiseAndFulfiller<void>();
	bool destroyed = false;
	auto destructionPromise = paf.promise.then([&]() { destroyed = true; }).eagerlyEvaluate(nullptr);

	{
		auto request = client.neverReturnRequest();
		request.setCap(kj::heap<TestCapDestructor>(kj::mv(paf.fulfiller)));

		{
			auto responsePromise = request.send();

			// Allow some time to settle.
			EXPECT_EQ(1u, client.getCallSequenceRequest().send().wait(waitScope).getN());
			EXPECT_EQ(2u, client.getCallSequenceRequest().send().wait(waitScope).getN());

			// The cap shouldn't have been destroyed yet because the call never returned.
			EXPECT_FALSE(destroyed);
		}
	}

	// Now the cap should be released.
	destructionPromise.wait(waitScope);
	EXPECT_TRUE(destroyed);

	cout << "Cancel test end" << endl;
}

void SendTwiceTest(capnp::EzRpcClient& rpc)
{
	cout << "SendTwice test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto client = rpc.getMain<TestMoreStuff>();

	auto paf = kj::newPromiseAndFulfiller<void>();
	bool destroyed = false;
	auto destructionPromise = paf.promise.then([&]() { destroyed = true; }).eagerlyEvaluate(nullptr);

	auto cap = TestInterface::Client(kj::heap<TestCapDestructor>(kj::mv(paf.fulfiller)));

	{
		auto request = client.callFooRequest();
		request.setCap(cap);

		EXPECT_EQ("bar", request.send().wait(waitScope).getS());
	}

	// Allow some time for the server to release `cap`.
	EXPECT_EQ(1, client.getCallSequenceRequest().send().wait(waitScope).getN());

	{
		// More requests with the same cap.
		auto request = client.callFooRequest();
		auto request2 = client.callFooRequest();
		request.setCap(cap);
		request2.setCap(kj::mv(cap));

		auto promise = request.send();
		auto promise2 = request2.send();

		EXPECT_EQ("bar", promise.wait(waitScope).getS());
		EXPECT_EQ("bar", promise2.wait(waitScope).getS());
	}

	// Now the cap should be released.
	destructionPromise.wait(waitScope);
	EXPECT_TRUE(destroyed);

	cout << "SendTwice test end" << endl;
}

RemotePromise<TestCallOrder::GetCallSequenceResults> getCallSequence(
	TestCallOrder::Client& client, uint expected) {
	auto req = client.getCallSequenceRequest();
	req.setExpected(expected);
	return req.send();
}

void EmbargoTest(capnp::EzRpcClient& rpc)
{
	cout << "Embargo test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto client = rpc.getMain<TestMoreStuff>();

	auto cap = TestCallOrder::Client(kj::heap<TestCallOrderImpl>());

	auto earlyCall = client.getCallSequenceRequest().send();

	auto echoRequest = client.echoRequest();
	echoRequest.setCap(cap);
	auto echo = echoRequest.send();

	auto pipeline = echo.getCap();

	auto call0 = getCallSequence(pipeline, 0);
	auto call1 = getCallSequence(pipeline, 1);

	earlyCall.wait(waitScope);

	auto call2 = getCallSequence(pipeline, 2);

	auto resolved = echo.wait(waitScope).getCap();

	auto call3 = getCallSequence(pipeline, 3);
	auto call4 = getCallSequence(pipeline, 4);
	auto call5 = getCallSequence(pipeline, 5);

	EXPECT_EQ(0, call0.wait(waitScope).getN());
	EXPECT_EQ(1, call1.wait(waitScope).getN());
	EXPECT_EQ(2, call2.wait(waitScope).getN());
	EXPECT_EQ(3, call3.wait(waitScope).getN());
	EXPECT_EQ(4, call4.wait(waitScope).getN());
	EXPECT_EQ(5, call5.wait(waitScope).getN());

	cout << "Embargo test end" << endl;
}

template <typename T>
void expectPromiseThrows(kj::Promise<T>&& promise, kj::WaitScope& waitScope) {
	EXPECT_TRUE(promise.then([](T&&) { return false; }, [](kj::Exception&&) { return true; })
		.wait(waitScope));
}

template <>
void expectPromiseThrows(kj::Promise<void>&& promise, kj::WaitScope& waitScope) {
	EXPECT_TRUE(promise.then([]() { return false; }, [](kj::Exception&&) { return true; })
		.wait(waitScope));
}

void EmbargoErrorTest(capnp::EzRpcClient& rpc)
{
	cout << "EmbargoError test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto client = rpc.getMain<TestMoreStuff>();

	auto paf = kj::newPromiseAndFulfiller<TestCallOrder::Client>();

	auto cap = TestCallOrder::Client(kj::mv(paf.promise));

	auto earlyCall = client.getCallSequenceRequest().send();

	auto echoRequest = client.echoRequest();
	echoRequest.setCap(cap);
	auto echo = echoRequest.send();

	auto pipeline = echo.getCap();

	auto call0 = getCallSequence(pipeline, 0);
	auto call1 = getCallSequence(pipeline, 1);

	earlyCall.wait(waitScope);

	auto call2 = getCallSequence(pipeline, 2);

	auto resolved = echo.wait(waitScope).getCap();

	auto call3 = getCallSequence(pipeline, 3);
	auto call4 = getCallSequence(pipeline, 4);
	auto call5 = getCallSequence(pipeline, 5);

	paf.fulfiller->rejectIfThrows([]() { KJ_FAIL_ASSERT("foo") { break; } });

	expectPromiseThrows(kj::mv(call0), waitScope);
	expectPromiseThrows(kj::mv(call1), waitScope);
	expectPromiseThrows(kj::mv(call2), waitScope);
	expectPromiseThrows(kj::mv(call3), waitScope);
	expectPromiseThrows(kj::mv(call4), waitScope);
	expectPromiseThrows(kj::mv(call5), waitScope);

	// Verify that we're still connected (there were no protocol errors).
	getCallSequence(client, 1).wait(waitScope);

	cout << "EmbargoError test end" << endl;
}

void EmbargoNullTest(capnp::EzRpcClient& rpc)
{
	cout << "EmbargoNull test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto client = rpc.getMain<TestMoreStuff>();

	auto promise = client.getNullRequest().send();

	auto cap = promise.getNullCap();

	auto call0 = cap.getCallSequenceRequest().send();

	promise.wait(waitScope);

	auto call1 = cap.getCallSequenceRequest().send();

	expectPromiseThrows(kj::mv(call0), waitScope);
	expectPromiseThrows(kj::mv(call1), waitScope);

	// Verify that we're still connected (there were no protocol errors).
	getCallSequence(client, 0).wait(waitScope);

	cout << "EmbargoNull test end" << endl;
}

void CallBrokenPromiseTest(capnp::EzRpcClient& rpc)
{
	cout << "CallBrokenPromise test start" << endl;

	auto& waitScope = rpc.getWaitScope();

	auto client = rpc.getMain<TestMoreStuff>();

	auto paf = kj::newPromiseAndFulfiller<TestInterface::Client>();

	{
		auto req = client.holdRequest();
		req.setCap(kj::mv(paf.promise));
		req.send().wait(waitScope);
	}

	bool returned = false;
	auto req = client.callHeldRequest().send()
		.then([&](capnp::Response<TestMoreStuff::CallHeldResults>&&) {
		returned = true;
	}, [&](kj::Exception&& e) {
		returned = true;
		kj::throwRecoverableException(kj::mv(e));
	}).eagerlyEvaluate(nullptr);

	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);

	EXPECT_FALSE(returned);

	paf.fulfiller->rejectIfThrows([]() { KJ_FAIL_ASSERT("foo") { break; } });

	expectPromiseThrows(kj::mv(req), waitScope);
	EXPECT_TRUE(returned);

	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);
	kj::evalLater([]() {}).wait(waitScope);

	// Verify that we're still connected (there were no protocol errors).
	getCallSequence(client, 1).wait(waitScope);

	cout << "CallBrokenPromise test end" << endl;
}

int main(int argc, const char* argv[])
{
	if (argc != 3) 
	{
		PrintUsage(argv);
		return 1;
	}

	if (strncmp(argv[1], "client", 6) == 0)
	{
		try
		{
			capnp::EzRpcClient client(argv[2], 33444);
			cout << "Connecting" << endl;

			auto& waitScope = client.getWaitScope();

			if (strcmp(argv[1], "client:Basic") == 0)
			{
				BasicTest(client);
			}
			else if (strcmp(argv[1], "client:Pipelining") == 0)
			{
				PipeliningTest(client);
			}
			else if (strcmp(argv[1], "client:Release") == 0)
			{
				ReleaseTest(client);
			}
			else if (strcmp(argv[1], "client:ReleaseOnCancel") == 0)
			{
				ReleaseOnCancelTest(client);
			}
			else if (strcmp(argv[1], "client:TailCall") == 0)
			{
				TailCallTest(client);
			}
			else if (strcmp(argv[1], "client:Cancelation") == 0)
			{
				CancelationTest(client);
			}
			else if (strcmp(argv[1], "client:PromiseResolve") == 0)
			{
				PromiseResolveTest(client);
			}
			else if (strcmp(argv[1], "client:RetainAndRelease") == 0)
			{
				RetainAndReleaseTest(client);
			}
			else if (strcmp(argv[1], "client:Cancel") == 0)
			{
				CancelTest(client);
			}
			else if (strcmp(argv[1], "client:SendTwice") == 0)
			{
				SendTwiceTest(client);
			}
			else if (strcmp(argv[1], "client:Embargo") == 0)
			{
				EmbargoTest(client);
			}
			else if (strcmp(argv[1], "client:EmbargoError") == 0)
			{
				EmbargoErrorTest(client);
			}
			else if (strcmp(argv[1], "client:EmbargoNull") == 0)
			{
				EmbargoNullTest(client);
			}
			else if (strcmp(argv[1], "client:CallBrokenPromise") == 0)
			{
				CallBrokenPromiseTest(client);
			}
		}
		catch (const std::exception& exception)
		{
			cerr << exception.what() << endl;
		}
	}
	else if (strncmp(argv[1], "server", 6) == 0)
	{
		int callCount = 0;
		Capability::Client mainInterface = nullptr;

		if (strcmp(argv[1], "server:Interface") == 0)
		{
			mainInterface = kj::heap<TestInterfaceImpl>(callCount);
		}
		else if (strcmp(argv[1], "server:Pipeline") == 0)
		{
			mainInterface = kj::heap<TestPipelineImpl>();
		}
		else if (strcmp(argv[1], "server:MoreStuff") == 0)
		{
			mainInterface = kj::heap<TestMoreStuffImpl>();
		}
		else if (strcmp(argv[1], "server:TailCaller") == 0)
		{
			mainInterface = kj::heap<TestTailCallerImpl>();
		}
		else
		{
			PrintUsage(argv);
			return 2;
		}

		capnp::EzRpcServer server(mainInterface, argv[2], 0);
		auto& waitScope = server.getWaitScope();

		uint port = server.getPort().wait(waitScope);
		cout << "Listening on port " << port << "..." << endl;

		kj::NEVER_DONE.wait(waitScope);
	}
	else
	{
		PrintUsage(argv);
		return 2;
	}
}
