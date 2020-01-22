using System;

namespace MsBuildGenerationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate some generated classes ensures that they are really present.
            // Note that this code is not supposed to test runtime behavior, we have plenty of other test cases for that purpose.

            void use(object x)
            {
            }

            var vatId = new Capnp.Rpc.Twoparty.VatId();
            use(vatId);
            var msg = new Capnp.Rpc.Message();
            use(msg);
            var node = new Capnp.Schema.Node();
            use(node);
            var x = Capnproto_test.Capnp.Test.TestEnum.garply;
            use(x);
            var imp = new CapnpGen.TestImport();
            use(imp);
            var imp2 = new CapnpGen.TestImport2();
            use(imp2);
            var book = new CapnpGen.AddressBook();
            use(book);
        }
    }
}
