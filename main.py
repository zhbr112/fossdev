from fastapi import FastAPI

items = ["pivo", "riba", "holodec"]

app = FastAPI()

@app.get("/")
def read_root():
    return {"message": "Hello, Pivozavrы!"}


@app.get("/items")
def get_items():
    return {"items": items}


@app.get("/item")
def get_item(id: int):
    return {"item": items[id]}
