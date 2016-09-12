TeensyTwitter
=============
[![Build Status](https://travis-ci.org/sjmelia/TeensyTwitter.svg)](https://travis-ci.org/sjmelia/TeensyTwitter)
[![nuget](https://img.shields.io/badge/nuget-v5.3.0-blue.svg)](https://www.nuget.org/packages/ArrayOfBytes.TeensyTwitter)

A minimal dotnetcore client library for the Twitter API - inspired by [TinyTwitter](https://github.com/jmhdez/TinyTwitter)
but updated for dotnetcore and with async support etc.

Usage
-----

Grab your oAuth credentials and create a `TwitterClient` instance; then go for it!

``` C#
using ArrayOfBytes.TeensyTwitter;

...

OAuthConfig config = new OAuthConfig(...);
TwitterClient client = new TwitterClient(config);
await client.UpdateStatus("Hello, World!");
```

See the example console project for a little more.