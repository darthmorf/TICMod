anchors = []

def parseHeaders(headerMarker, headerStyleStart, headerStyleEnd, content):
    global anchors
    for i in range(len(content)):
        if content[i].startswith(headerMarker):
            content[i] = content[i].replace(headerMarker, "")
            anchor = content[i].replace(" ", "-").lower()
            anchors.append(anchor)
            content[i] = "[anchor]" + anchor + "[/anchor]" + content[i]
            content[i] = headerStyleStart + content[i] + headerStyleEnd

    return content

def parseCode(content):
    for i in range(len(content)):
        while "`" in content[i]:
            content[i] = content[i].replace("`", "[ICODE]", 1)
            content[i] = content[i].replace("`", "[/ICODE]", 1)
    return content

def parseAnchors(content):
    global anchors
    for i in range(len(content)):
        for anchor in anchors:
            if anchor in content[i]:
                content[i] = content[i].replace("(#" + anchor + ")", "")
                title = content[i].replace("-", "").replace("[", "").replace("]", "").strip()
                print(title)
                content[i] = content[i].replace("[" + title + "]", "[goto=" + anchor + "]" + title + "[/goto]")
                break
    return content    
    
def rebuildContent(content):
    rebuilt = ""
    for line in content:
        rebuilt += line + "\n"
    return rebuilt


data = None

with open("./README.md") as file: 
   data = file.read()

data = data.replace("<br/>", "")
data = data.replace("<br>", "")
data = data.replace("<!-- omit in toc -->", "")
readme = data.split("\n")
readme = parseHeaders("## ", "[SIZE=7][B]", "[/B][/SIZE]", readme)
readme = parseHeaders("### ", "[SIZE=6][B]", "[/B][/SIZE]", readme)
readme = parseHeaders("#### ", "[SIZE=5][B]", "[/B][/SIZE]", readme)
readme = parseCode(readme)
readme = parseAnchors(readme)
readme = rebuildContent(readme)

print(readme)
with open("./README.bb", "w") as file: 
   file.write(readme)
