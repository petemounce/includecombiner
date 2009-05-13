# IncludeCombiner

[IncludeCombiner on my blog](http://blog.neverrunwithscissors.com/tag/include-combiner)

## Description

IncludeCombiner is a resuable component written for ASP.NET MVC that is for handling static file-includes like CSS and Javascript in a best-practise way, that can be dropped into your own ASP.NET MVC web project, configured, and used with a minimum of effort.

It:

* Mashes together the includes on a page to a single request, following YSlow! guideline for same (minimise number of requests necessary to load page)
* Minifies the include-combination (using the YUICompressor); strips comments and whitespace to reduce file-weight on the wire
* If the browser accepts compression, compresses (gzip/deflate) the response
* Appends cache-policy headers to the response to control client-side caching
* Does all this at run-time with no configuration or build complexity overhead

Also:

* (TODO) It's configurable so you can control:

	* The path that the combiner-controller's action happens at, per include-type (CSS/JS)
	* The minification options, per include-type
	* The default length of time to cache the include-combination at the client-side for
	* Which compression to pick when a choice is accepted

* There's a DebugFilterAttribute responsible for switching operation between debug and release-mode, and an HtmlHelper extension method to test whether we're in debug-mode (it reacts to a debug=1 or debug=0 in the querystring, and sets a debug cookie if debug=1; removes it if debug=0)
* It's pluggable, so you can replace collaborating components (for example if you wanted to replace the in-memory static storage with a cache-backed IIncludeStorage)
* It has a demo
* It has benchmarks (see Demo.Site)
* It's highly covered in unit-tests - see http://teamcity.codebetter.com -> IncludeCombiner
