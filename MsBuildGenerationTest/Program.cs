using System;

namespace MsBuildGenerationTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate some generated classes ensures that they are really present.
            // Note that this code is not supposed to test runtime behavior, we have plenty of other test cases for that purpose.

            var vatId = new Capnp.Rpc.Twoparty.VatId();
            var msg = new Capnp.Rpc.Message();
            var node = new Capnp.Schema.Node();
            var x = Capnproto_test.Capnp.Test.TestEnum.garply;
            var imp = new CapnpGen.TestImport();
            var imp2 = new CapnpGen.TestImport2();
            var book = new CapnpGen.AddressBook();
        }
    }
}
