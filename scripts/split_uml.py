#!/usr/bin/env python3
import re, os
import networkx as nx

# Rutas de entrada y salida
INPUT = 'UML/include.puml'
OUTDIR = 'UML/components'

# Asegura que exista la carpeta de salida
os.makedirs(OUTDIR, exist_ok=True)

# Lee todas las líneas de include.puml
with open(INPUT, encoding='utf-8') as f:
    lines = f.read().splitlines()

# Separa definiciones y relaciones
defs = [l for l in lines if re.match(r'\s*(class|interface|enum|struct)\b', l)]
rels = [l for l in lines if re.search(r'-->|o--|\*--|\.\.>', l)]

# Extrae todos los nombres de tipos
names = set()
for l in defs:
    m = re.match(r'\s*(?:class|interface|enum|struct)\s+"?([^ "\n]+)', l)
    if m:
        names.add(m.group(1))

# Construye un grafo con networkx
G = nx.Graph()
G.add_nodes_from(names)
for l in rels:
    m = re.match(r'\s*"?(?P<a>[^ "\n]+)"?\s+[-.\*o]+>+\s+"?(?P<b>[^ "\n]+)"?', l)
    if m and m.group('a') in names and m.group('b') in names:
        G.add_edge(m.group('a'), m.group('b'))

# Para cada componente conectado, genera su propio .puml
for i, comp in enumerate(nx.connected_components(G), start=1):
    out_path = f'{OUTDIR}/comp_{i}.puml'
    with open(out_path, 'w', encoding='utf-8') as out:
        out.write('@startuml\n')
        # Definiciones
        for d in defs:
            if any(re.search(r'\b'+re.escape(n)+r'\b', d) for n in comp):
                out.write(d + '\n')
        out.write('\n')
        # Relaciones
        for r in rels:
            if any(re.search(r'\b'+re.escape(n)+r'\b', r) for n in comp):
                out.write(r + '\n')
        out.write('@enduml\n')
    print(f'Wrote {out_path}')
