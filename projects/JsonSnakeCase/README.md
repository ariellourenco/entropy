# Snake Case

In the .NET, memory can be allocated in one of two places: on the stack or on the heap. The exact rules of where objects are stored is complicated.

In a typical .NET application, most of the object created will be reference types, and stored on the heap. Heap memory is garbage collected, which means more objects you create, the more pressure you put on the garbage collector. ```string```s are a common source of GC pressure in .NET applications, especially web applications. The "heap-based but immutable" nature of strings mean you typically create a lot of them, and they all need to be collected by the GC. This is especially true when creating strings from multiple other values and sources, for example serializing/deserializing an object.

This project is case study on how to reduce memory allocation and improve performance by using [ArrayPool<T>](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1?view=net-5.0) in serialization and deserialization processes. It adds support for **Snake case** (stylized as snake_case), which refers to the style of writing in which each space is replaced by an underscore (_) character, and the first letter of each word written in lowercase. Snake case naming is pretty common, especially in the Ruby world, and probably a few popular example using this naming convention are the Github, Facebook, AWS, and Twitter APIs.

> ```ArrayPool``` can be used to reduce the overall allocations in your application where arrays are frequently created and destroyed. Renting a buffer supplies the caller with a ```char[]``` from the heap, but you must be sure to Return it later to avoid memory leaks.

### The **Snake case** naming implementation adhere to the following capitalization rules:

* Ignores leading/trailing whitespace.
* Collapse multiple underscores into one.
* Numbers are only treated as separate words if followed by capital letter.
* Sequence of consecutive capital letter is considered one "word"
* Ignore punctuation.

### The code also has a benchmark that compares other implementations.

### Acknowledgement
  
Most of the code has been inspired by the conversation on the threads below:

* https://github.com/dotnet/runtime/issues/782
* https://github.com/dotnet/corefx/pull/41354
* https://github.com/dotnet/runtime/pull/54128

### References

* [A deep dive on StringBuilder](https://andrewlock.net/a-deep-dive-on-stringbuilder-part-1-the-overall-design-and-first-look-at-the-internals/)
* [Pooling large arrays with ArrayPool](https://adamsitnik.com/Array-Pool/)
* [Large Object Heap Uncovered](https://devblogs.microsoft.com/dotnet/large-object-heap-uncovered-from-an-old-msdn-article/)
