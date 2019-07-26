:: создать проект библиотеку
dotnet new classlib -n EvenOdd -lang f#
:: создать проект с тестами
dotnet new Xunit -n EvenOdd.Tests -lang f#
:: уставновить шаблон проекта жирафа
dotnet new -i "giraffe-template::*"
:: создать проект жирафа
dotnet new giraffe -n EvenOdd.Api --ViewEngine none