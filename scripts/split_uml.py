#!/usr/bin/env python3
import re, os
import networkx as nx

INPUT = 'UML/include.puml'
OUTDIR = 'UML/components'
os.makedirs(OUTDIR, exist_ok=True)
lines = open(INPUT, encoding='utf-8').read().splitlines()

# Relaciones idénticas al split actual
rels = [l for l in lines if re.search(r'-->|o--|\*--|\.\.>', l)]

# Extraer todos los nombres de tipos
defs_all = []
for idx, l in enumerate(lines):
    m = re.match(r'\s*(class|interface|enum|struct)\s+"?([^ "\n{]+)', l)
    if m:
        defs_all.append((idx, m.group(2)))  # guardamos línea e nombre

# Construir grafo
G = nx.Graph()
names = {name for _, name in defs_all}
G.add_nodes_from(names)
for l in rels:
    m = re.match(r'\s*"?(?P<a>[^ "\n]+)"?\s+[-.\*o]+>+\s+"?(?P<b>[^ "\n]+)"?', l)
    if m and m.group('a') in names and m.group('b') in names:
        G.add_edge(m.group('a'), m.group('b'))

# Para cada componente, extraer bloques completos de definición
for i, comp in enumerate(nx.connected_components(G), start=1):
    # ignorar si no hay aristas
    sub = G.subgraph(comp)
    if sub.number_of_edges() == 0:
        continue

    out_path = f'{OUTDIR}/comp_{i}.puml'
    with open(out_path, 'w', encoding='utf-8') as out:
        out.write('@startuml\n')

        # recorremos defs_all buscando los que están en comp
        for idx, name in defs_all:
            if name not in comp:
                continue
            # arrancamos el bloque desde lines[idx]
            brace_count = 0
            for j in range(idx, len(lines)):
                line = lines[j]
                out.write(line + '\n')
                # contar llaves para detectar fin de bloque
                brace_count += line.count('{') - line.count('}')
                if brace_count == 0:
                    break

        out.write('\n')
        # relaciones
        for r in rels:
            if any(re.search(r'\b'+re.escape(n)+r'\b', r) for n in comp):
                out.write(r + '\n')
        

        brace_balance = 0
        for idx, name in defs_all:
            if name in comp:
                # contamos en el bloque de definición
                for k in range(idx, len(lines)):
                    line = lines[k]
                    brace_balance += line.count('{') - line.count('}')
                    if brace_balance <= 0:
                        break
        for r in rels:
            if any(re.search(r'\b'+re.escape(n)+r'\b', r) for n in comp):
                brace_balance += r.count('{') - r.count('}')
        # Inyecta las llaves de cierre que falten
        for _ in range(brace_balance):
            out.write('}\n')

        out.write('@enduml\n')
    print(f'Wrote {out_path}')
