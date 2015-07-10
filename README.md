# LeadPipe.Net.NHibernateExamples

This project demonstrates common NHibernate alerts and anti-patterns with solutions.

## Setup

To run these examples you'll need to:

- Clone the repository
- Download and install the [NHibernate Profiler](http://www.hibernatingrhinos.com/products/nhprof)
- Add a reference to the HibernatingRhinos.Profiler.Appender.dll assembly which is provided with the NHibernate Profiler

Each concept is described in an individual file containing NUnit unit tests. Start with the method named Problem and then run the subsequent unit tests to see the impact of different solutions. The code itself is heavily documented, but if you have questions please ask!

## Excellent Reading

- [Eric Lippert's Guidelines and Rules for GetHashCode](http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
- [NHibernate Documentation](http://nhibernate.info/doc/nh/en/index.html)
- [NHibernate Source Code](https://github.com/nhibernate/nhibernate-core/tree/master/src)
- [NHibernate Profiler Learning Documentation](http://hibernatingrhinos.com/Products/NHProf/learn)

