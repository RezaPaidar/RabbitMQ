# RabbitMQ Priority Queue Manager

A robust .NET application demonstrating RabbitMQ priority queue functionality with dynamic queue management, batch operations, and clean architecture.

---

## Features

- **Priority Queue Support:** Messages with higher priority are processed first  
- **Dynamic Queue Management:** Create, purge, and delete queues programmatically  
- **Batch Operations:** Send multiple messages with different priorities  
- **Clean Architecture:** Separation of concerns with interfaces and services  
- **Flexible Configuration:** Configure multiple queues with different settings  
- **Durable Queues:** Messages persist across broker restarts  
- **Type Safety:** Generic methods for safe message handling  

---

## Queue Structure

The application manages three main queues:

- **Qx:** Queue processing tasks  
- **Qy:** Queue tasks and services  
- **Qz:** Queue alerts and critical operations  

All queues support message priorities **0–255**, with higher numbers meaning higher priority.

---

## Installation

### **Prerequisites**
- .NET **8.0 SDK**  
- RabbitMQ **3.8+** (with priority queue support)  
- `RabbitMQ.Client` **7.1.2+**

---

### **Setup**

#### Install RabbitMQ.Client
```bash
dotnet add package RabbitMQ.Client --version 7.1.2
