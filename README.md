# Web API Development with ASP.NET Core 6

## Justification for migrating WCF SOAP services over to a modern RESTful Web API

- SOAP-based services use an XML schema that can lead to slower performance compared to a JSON-based API.
- WCF SOAP services require complex configuration that can lead to more bugs and security vulnerabilities compared to the much simpler configuration required for a modern Web API.
- Microsoft has no plans to bring WCF forward to a newer version of .NET. Although Microsoft will continue to support WCF with bug fixes and security patches, they do not plan to add new features or improve performance.
- A Web API can take advantage of the features included with more recent versions of .NET. These features include cross-platform capabilities, containerization support, a newer version of the C# language, and much more. This can lead to easier maintenance, increased developer productivity, better performance, reduced deployment costs, and improved security.
- Because of its simpler protocol, a Web API is much easier to test for correctness and security by using a wider variety of tools.
- The vast majority of large-scale web services today are implemented as RESTful Web APIs rather than SOAP services. Some examples include Amazon AWS, Microsoft Azure, Google Cloud Platform, and the data services provided by many government agencies (including the FAA, FDA, GSA, and DOJ).      
