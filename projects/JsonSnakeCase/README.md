# Snake Case

In the .NET, memory can be allocated in one of two places: on the stack or on the heap.

In a typical .NET application, most of the object created will be reference types, and stored on the heap. Heap memory is garbage collected, which means more objects you create, the more pressure you put on the garbage collector. ```string```s are a common source of GC pressure in .NET applications, especially web applications. The "heap-based but immutable" nature of strings mean you typically create a lot of them, and they all need to be collected by the GC. This is especially true when creating strings from multiple other values and sources, for example serializing/deserializing an object.

This project is case study on how to reduce memory allocation and improve performance by using [ArrayPool<T>](https://docs.microsoft.com/en-us/dotnet/api/system.buffers.arraypool-1?view=net-5.0) in serialization and deserialization processes. It adds support for **Snake case** (stylized as snake_case), which refers to the style of writing in which each space is replaced by an underscore (_) character, and the first letter of each word written in lowercase. Snake case naming is pretty common, especially in the Ruby world, and probably a few popular example using this naming convention are the Github, Facebook, AWS, and Twitter APIs.

> ```ArrayPool``` can be used to reduce the overall allocations in your application where arrays are frequently created and destroyed. Renting a buffer supplies the caller with a ```char[]``` from the heap, but you must be sure to Return it later to avoid memory leaks.

### The **Snake case** naming implementation adhere to the following capitalization rules:

* Ignores leading/trailing whitespace.
* Collapse multiple underscores into one.
* Numbers are only treated as separate words if followed by capital letter.
* Sequence of consecutive capital letter is considered one "word".
* Ignore any non-letter or non-digit characters and treat them as word boundaries.

### The code also has a benchmark that compares other implementations.

#### Snake Case vs Built-In Camel Case

```
BenchmarkDotNet=v0.13.0, OS=macOS Big Sur 11.5.2 (20G95) [Darwin 20.6.0]
Intel Core i9-9980HK CPU 2.40GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK=5.0.201
  [Host]     : .NET 5.0.4 (5.0.421.11614), X64 RyuJIT
  DefaultJob : .NET 5.0.4 (5.0.421.11614), X64 RyuJIT


|  Method |     Mean |     Error |    StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------- |---------:|----------:|----------:|------:|--------:|-----:|-------:|------:|------:|----------:|
| Default | 1.002 us | 0.0146 us | 0.0137 us |  1.00 |    0.00 |    1 | 0.0706 |     - |     - |     600 B |
|   Camel | 1.376 us | 0.0189 us | 0.0167 us |  1.37 |    0.03 |    2 | 0.1183 |     - |     - |     992 B |
|   Snake | 2.745 us | 0.0213 us | 0.0199 us |  2.74 |    0.04 |    3 | 0.1411 |     - |     - |   1,184 B |
```

### Acknowledgement
  
Most of the code has been inspired by the conversation on the threads below:

* https://github.com/dotnet/runtime/issues/782
* https://github.com/dotnet/corefx/pull/41354
* https://github.com/dotnet/runtime/pull/54128

### References

* [A deep dive on StringBuilder](https://andrewlock.net/a-deep-dive-on-stringbuilder-part-1-the-overall-design-and-first-look-at-the-internals/)
* [Pooling large arrays with ArrayPool](https://adamsitnik.com/Array-Pool/)
* [Large Object Heap Uncovered](https://devblogs.microsoft.com/dotnet/large-object-heap-uncovered-from-an-old-msdn-article/)
