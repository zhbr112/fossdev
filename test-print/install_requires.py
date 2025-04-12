import json
from os import path

here = path.abspath(path.dirname(__file__))

with open(path.join(here, "README.rst"), encoding="utf-8") as f:
    long_description = f.read()


def read_dependencies(fname):
    filepath = path.join(here, fname)
    with open(filepath) as piplock:
        content = json.load(piplock)
        return [dependency for dependency in content.get("default")]
