I decided not to use interfaces for services, because:
- services in this app are quite simple
- services in this app won't have any alternative implementations
- I didn't make any unit tests, so there's no need to mock them

I decided not to use DTOs (Data Transfer Objects),
because database entities currently don't contain any information
that should be hidden from the client.

