# dot-net-st-pete-api

.net core api with JWT simple authentication

## macOS pre-requisities

#### [Install Homebrew](https://brew.sh/) 
`/usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"`

#### Update [Homebrews](http://brew.sh/) package database
`brew update`

#### Install the MongoDB Binaries
`brew install mongodb`

#### Create the data directory
`mkdir -p /data/db`

#### Install .NET Core Dependencies
`brew install openssl`

`mkdir -p /usr/local/lib`

`ln -s /usr/local/opt/openssl/lib/libcrypto.1.0.0.dylib /usr/local/lib/`

`ln -s /usr/local/opt/openssl/lib/libssl.1.0.0.dylib /usr/local/lib/`

[Download .NET Core SDK](https://go.microsoft.com/fwlink/?LinkID=835011)

## Usage
* Fork this repo
* Clone this repo

### CD to project directory
`cd dot-net-st-pete-api`

### Start a local Mongo instance (using data directory from above)
`mongod --dbpath /data/db`

### Restore .NET core dependencies
`dotnet restore`

### Start .NET core API
`dotnet run`
