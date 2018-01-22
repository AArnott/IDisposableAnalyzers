#### 0.1.4.3
* BUGFIX: IDISP004 handle CompositeDisposable.Add.

#### 0.1.4
* PERFORMANCE: Merged many analyzers doing the same expensive analysis to fewer.
* BUGFIX: IDISP004 nag on argument to invcation.
* BUGFIX: IDISP003 handle lazy properties.
* BUGFIX: Generate correct code for disposing explicit disposable.
* BUGFIX: IDISP004 nag on chained invocation.
* BUGFIX: IDISP004 default warning.
* FEATURE: IDISP0011 don't return disposed instance.
* FEATURE: IDISP0011 property should not return created disposable.

#### 0.1.3.1
* BUGFIX: Don't nag about implementing IDisposable if disposing in teardown
* FEATURE: Codefix suggesting generate dispose in teardown for dispose before reassigning.

#### 0.1.3
* BUGFIX: Handle dispose in setter.
* FEATURE: Handle Setup & TearDown

#### 0.1.2
* BUGFIXES: whitespace in codegen.
* BUGFIX: Override public virtual Dispose