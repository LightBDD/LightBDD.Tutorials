## Summary

This tutorial shows how to convert regular unit tests to LightBDD scenarios.
It covers basics how to install and enable LightBDD, write steps and share state between them as well as execution progress and reporting.
It bases on xunit test framework, but it is applicable to any framework LightBDD integrates with.

## Converting unit tests to LightBDD

In this tutorial I would like to present how to convert regular tests into LightBDD scenarios.

For this purpose I have created a sample %%link{CustomerManagement}%% project for managing customers. It exposes interface %%interface{ICustomerService}%% that allows to create customers and retrieve them by ID and %%link{CustomerService|CustomerManagement/CustomerService.cs}%% class implementing it.

The %%link{CustomerManagement.Tests}%% project contains tests for this class written with xunit test framework that looks as follows: %%class{CustomerServiceTests}%%


