(window.webpackJsonp=window.webpackJsonp||[]).push([[16],{377:function(t,a,s){t.exports=s.p+"assets/img/launch-wpf-app.20481e49.gif"},402:function(t,a,s){"use strict";s.r(a);var n=s(42),e=Object(n.a)({},(function(){var t=this,a=t.$createElement,n=t._self._c||a;return n("ContentSlotsDistributor",{attrs:{"slot-key":t.$parent.slotKey}},[n("h1",{attrs:{id:"getting-start-for-wpf"}},[n("a",{staticClass:"header-anchor",attrs:{href:"#getting-start-for-wpf"}},[t._v("#")]),t._v(" Getting start for WPF")]),t._v(" "),n("h2",{attrs:{id:"create-a-project"}},[n("a",{staticClass:"header-anchor",attrs:{href:"#create-a-project"}},[t._v("#")]),t._v(" Create a project")]),t._v(" "),n("ul",[n("li",[t._v("Create a WPF App (.NET Framework) project.\n"),n("ul",[n("li",[t._v("Have to use .NET Framework 4.6.1 or later, or .NET Core 3.0 or later.")])])]),t._v(" "),n("li",[t._v("Install ReactiveProperty.WPF package from NuGet.")])]),t._v(" "),n("h2",{attrs:{id:"edit-codes"}},[n("a",{staticClass:"header-anchor",attrs:{href:"#edit-codes"}},[t._v("#")]),t._v(" Edit codes")]),t._v(" "),n("ul",[n("li",[t._v("Add Startup event handler on App class.")]),t._v(" "),n("li",[t._v("Add code to initialize scheduler for ReactiveProperty classes.")])]),t._v(" "),n("div",{staticClass:"language-csharp extra-class"},[n("pre",{pre:!0,attrs:{class:"language-csharp"}},[n("code",[n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Collections"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Generic")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Configuration")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Data")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Linq")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Threading"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Tasks")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Windows")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("Reactive"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Bindings")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("Reactive"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Bindings"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Schedulers")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("namespace")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("WpfApp1")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("partial")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("class")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("App")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(":")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token type-list"}},[n("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("Application")])]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("private")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token return-type class-name"}},[n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("void")])]),t._v(" "),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("Application_Startup")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token class-name"}},[n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("object")])]),t._v(" sender"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(",")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("StartupEventArgs")]),t._v(" e"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n            ReactivePropertyScheduler"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("SetDefault")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("new")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token constructor-invocation class-name"}},[t._v("ReactivePropertyWpfScheduler")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("Dispatcher"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n")])])]),n("ul",[n("li",[t._v("Create a MainWindowViewModel.cs file.")]),t._v(" "),n("li",[t._v("Edit files like following.")])]),t._v(" "),n("p",[t._v("MainWindowViewModel.cs")]),t._v(" "),n("div",{staticClass:"language-csharp extra-class"},[n("pre",{pre:!0,attrs:{class:"language-csharp"}},[n("code",[n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("Reactive"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Bindings")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("ComponentModel")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Linq")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("using")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("System"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Reactive"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),t._v("Linq")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n\n"),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("namespace")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("WpfApp1")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("class")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("MainWindowViewModel")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(":")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token type-list"}},[n("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("INotifyPropertyChanged")])]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("event")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token class-name"}},[t._v("PropertyChangedEventHandler")]),t._v(" PropertyChanged"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n\n        "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token return-type class-name"}},[t._v("ReactiveProperty"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("string")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v(" Input "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("get")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token return-type class-name"}},[t._v("ReadOnlyReactiveProperty"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("string")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v(" Output "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("get")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n\n        "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("public")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("MainWindowViewModel")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("{")]),t._v("\n            Input "),n("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("new")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token constructor-invocation class-name"}},[t._v("ReactiveProperty"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),n("span",{pre:!0,attrs:{class:"token keyword"}},[t._v("string")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token string"}},[t._v('""')]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n            Output "),n("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=")]),t._v(" Input\n                "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("Delay")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("TimeSpan"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("FromSeconds")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token number"}},[t._v("1")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v("\n                "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("Select")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),t._v("x "),n("span",{pre:!0,attrs:{class:"token operator"}},[t._v("=>")]),t._v(" x"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("ToUpper")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),t._v("\n                "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(".")]),n("span",{pre:!0,attrs:{class:"token function"}},[t._v("ToReadOnlyReactiveProperty")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("(")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(")")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(";")]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("}")]),t._v("\n")])])]),n("p",[t._v("MainWindow.xaml")]),t._v(" "),n("div",{staticClass:"language-xml extra-class"},[n("pre",{pre:!0,attrs:{class:"language-xml"}},[n("code",[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("Window")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("x:")]),t._v("Class")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("WpfApp1.MainWindow"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("xmlns")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("http://schemas.microsoft.com/winfx/2006/xaml/presentation"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("xmlns:")]),t._v("x")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("http://schemas.microsoft.com/winfx/2006/xaml"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("xmlns:")]),t._v("d")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("http://schemas.microsoft.com/expression/blend/2008"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("xmlns:")]),t._v("mc")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("http://schemas.openxmlformats.org/markup-compatibility/2006"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("xmlns:")]),t._v("local")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("clr-namespace:WpfApp1"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("mc:")]),t._v("Ignorable")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("d"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Title")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("MainWindow"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Height")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("350"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Width")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("525"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("Window.DataContext")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),n("span",{pre:!0,attrs:{class:"token namespace"}},[t._v("local:")]),t._v("MainWindowViewModel")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("/>")])]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("</")]),t._v("Window.DataContext")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("StackPanel")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("Label")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Content")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("Input"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("/>")])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("TextBox")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Text")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("{Binding Input.Value, UpdateSourceTrigger=PropertyChanged}"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n                 "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Margin")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("5"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("/>")])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("Label")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Content")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("Output"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("/>")])]),t._v("\n        "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("<")]),t._v("TextBlock")]),t._v(" "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Text")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("{Binding Output.Value}"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v("\n                   "),n("span",{pre:!0,attrs:{class:"token attr-name"}},[t._v("Margin")]),n("span",{pre:!0,attrs:{class:"token attr-value"}},[n("span",{pre:!0,attrs:{class:"token punctuation attr-equals"}},[t._v("=")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')]),t._v("5"),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v('"')])]),t._v(" "),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("/>")])]),t._v("\n    "),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("</")]),t._v("StackPanel")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v("\n"),n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token tag"}},[n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v("</")]),t._v("Window")]),n("span",{pre:!0,attrs:{class:"token punctuation"}},[t._v(">")])]),t._v("\n")])])]),n("h2",{attrs:{id:"launch-the-application"}},[n("a",{staticClass:"header-anchor",attrs:{href:"#launch-the-application"}},[t._v("#")]),t._v(" Launch the application.")]),t._v(" "),n("p",[t._v("After launching the app, you can see the below window.\nThe output value was displayed to upper case, after 1sec from the input.")]),t._v(" "),n("p",[n("img",{attrs:{src:s(377),alt:"Launch the app"}})])])}),[],!1,null,null,null);a.default=e.exports}}]);