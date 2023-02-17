---
order: 0
title:
  zh-CN: 异常处理
  en-US: Error handler
---

## zh-CN

异常统一处理：
1. `balzor`生命周期内的异常，无法处理，直接传递到`ErrorBoundry`处理；
2. 非MASA Blazor组件产生的异常，无法处理，直接传递到`ErrorBoundry`处理；
3. MASA Blazor组件非生命周期方法产生的异常，都可以处理，默认展示`Exception.Message`，也可以配置其它选项显示异常堆栈或自定义处理异常

## en-US

Exception unified handling:
1. Exceptions in the life cycle of `balzor` cannot be handled, and are directly passed to `ErrorBoundry` for processing;
2. Exceptions generated by non-MASA Blazor components cannot be handled and are directly passed to `ErrorBoundry` for processing;
3. Exceptions generated by non-lifecycle methods of MASA Blazor components can be handled. By default, `Exception.Message` is displayed. You can also configure other options to display exception stack or customize exception handling.