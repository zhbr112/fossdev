from fastapi import FastAPI

app = FastAPI()

@app.get("/")
def read_root():
    return {"message": "Hello, Pivozavrы!"}


@app.get("/items")
def get_items():
    return {"items": ["pivo", "riba", "holodec"]}
