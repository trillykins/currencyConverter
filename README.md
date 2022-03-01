# currencyConverter

API for converting currencies

## Installation

Build a docker container with the application:

`docker build -t currency-converter .`

Run the docker container:

`docker run -d -p 8080:80 --name CurrencyConverter currency-converter`

*If using Visual Studio for local development simply choose the Docker or IIS Express run configuration and start the application.*

## Usage

The REST API has the following two endpoints

Get a list of all available currencies:

* http://localhost:8080/currency

Convert an amount of currency from one currency to another (e.g. $1200 USD in JPY)

* http://localhost:8080/currency/convert?currencyFrom=USD&currencyTo=JPY&value=1200
