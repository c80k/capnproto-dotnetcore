@0xcda70121571f3166;

interface Issue22 {
	methodA @0 (param1 :Int64) -> ();
}
	   
interface Issue22Proxy {
	methodB @0 (param1 :Int64) -> (a :Issue22);
}

interface Issue22Skeleton {
	methodB @0 (param1 :Int64) -> (a :Issue22);
}
