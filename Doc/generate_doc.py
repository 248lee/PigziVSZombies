import os
import re
from bs4 import BeautifulSoup

# Directory containing the C# files
input_dir = r"D:\UnityProjects\GPTester\Assets\Scripts"
output_dir = "HTML"  # Directory to save HTML files
os.makedirs(output_dir, exist_ok=True)

# Regex patterns
class_pattern = re.compile(r"class\s+(\w+)")
method_pattern = re.compile(r"(public|protected|private)\s+\w+\s+(\w+)\s*\(.*?\)")
attribute_pattern = re.compile(r"(public|protected|private)\s+\w+\s+(\w+);")
xml_comment_pattern = re.compile(r"///\s*(.*)")

# Function to parse a single C# file
def parse_csharp_file(filepath):
    try:
        with open(filepath, "r") as file:
            lines = file.readlines()
    except Exception as e:
        print(f"Error reading file: {filepath}")
        print(f"Exception: {e}")
        return {}  # Return an empty dictionary if an error occurs

    classes = {}
    current_class = None
    current_comment = []

    for line in lines:
        line = line.strip()
        
        # Capture XML comments
        if line.startswith("///"):
            current_comment.append(xml_comment_pattern.match(line).group(1))
            continue

        # Identify class definitions
        class_match = class_pattern.search(line)
        if class_match:
            current_class = class_match.group(1)
            classes[current_class] = {"methods": [], "attributes": [], "comments": "\n".join(current_comment)}
            current_comment = []
            continue

        # Identify methods
        method_match = method_pattern.search(line)
        if method_match and current_class:
            modifier, name = method_match.groups()
            classes[current_class]["methods"].append({"name": name, "modifier": modifier, "comment": "\n".join(current_comment)})
            current_comment = []
            continue

        # Identify attributes
        attribute_match = attribute_pattern.search(line)
        if attribute_match and current_class:
            modifier, name = attribute_match.groups()
            classes[current_class]["attributes"].append({"name": name, "modifier": modifier, "comment": "\n".join(current_comment)})
            current_comment = []

    return classes

# Generate HTML for a single class
def generate_class_html(class_name, class_data):
    soup = BeautifulSoup("<html><body></body></html>", "html.parser")
    body = soup.body
    body.append(soup.new_tag("h1", string=class_name))
    
    if class_data["comments"]:
        body.append(soup.new_tag("p", string=f"Description: {class_data['comments']}"))
    
    body.append(soup.new_tag("h2", string="Attributes"))
    attr_list = soup.new_tag("ul")
    for attr in class_data["attributes"]:
        li = soup.new_tag("li")
        li.string = f"{attr['modifier']} {attr['name']} - {attr['comment']}"
        attr_list.append(li)
    body.append(attr_list)
    
    body.append(soup.new_tag("h2", string="Methods"))
    method_list = soup.new_tag("ul")
    for method in class_data["methods"]:
        li = soup.new_tag("li")
        li.string = f"{method['modifier']} {method['name']}() - {method['comment']}"
        method_list.append(li)
    body.append(method_list)
    
    return str(soup)

# Generate home page HTML
def generate_home_html(classes):
    soup = BeautifulSoup("<html><body></body></html>", "html.parser")
    body = soup.body
    body.append(soup.new_tag("h1", string="C# Documentation"))
    
    class_list = soup.new_tag("ul")
    for class_name in classes:
        li = soup.new_tag("li")
        a = soup.new_tag("a", href=f"{class_name}.html")
        a.string = class_name
        li.append(a)
        class_list.append(li)
    body.append(class_list)
    
    return str(soup)

# Main function to parse and generate documentation
def generate_documentation(input_dir, output_dir):
    all_classes = {}
    for root, _, files in os.walk(input_dir):
        for file in files:
            if file.endswith(".cs"):
                filepath = os.path.join(root, file)
                classes = parse_csharp_file(filepath)
                all_classes.update(classes)
    
    # Generate class HTML files
    for class_name, class_data in all_classes.items():
        class_html = generate_class_html(class_name, class_data)
        with open(os.path.join(output_dir, f"{class_name}.html"), "w", encoding="utf-8") as f:
            f.write(class_html)
    
    # Generate home page
    home_html = generate_home_html(all_classes)
    with open(os.path.join(output_dir, "index.html"), "w", encoding="utf-8") as f:
        f.write(home_html)

# Run the script
generate_documentation(input_dir, output_dir)
