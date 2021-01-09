# RoslyJump

RoslyJump is a free extension for Visual Studio 2019 for mouseless code navigation. For now, it only has limited support of C# 8.0.

## License

All code in this project is covered under the Apache 2 license. You can find a copy of it inside the same folder inside License.txt.

## QA

### Can I set the default hotkeys to a different one?

Yes. Go to `Tools > Options > Environment > Keyboard`. In the "Show commands containing" type "RoslyJump" you'll see all commands supported by the extension and be able to configure them..

### How to remember all specific jump cases in RoslyJump?

You don't have to remember them all. There are only two jump verticals - up/down and left/right. The up/down vertical has a single corresponding hotkey for each action:
    1. Jump Up/Down along a Syntax Tree

The left\right vertical has two corresponding hotkeys per each direction:

    1. Jump to next/previous node of the same type
    2. Jump to next/previous sibling node of a different type

When you are not sure which hotkey will work in a particular context, try every hotkey in the following order:

    1. Jump Next/Previous
    2. Jump Next/Previous Sibling
    3. Jump Up/Down

For example, when the cursor is set inside a method body, and you'd like to jump to another method press:

    1. `jump-context-up` hotkey until the entire method is highlighted
    2. `jump-next` hotkey to switch to another method

> Notice: `jump-to-next-sibling` hotkey, while the cursor is set on a method declaration will switch to the first field, property, event, etc in the same class

#### Jump To Next/Previous Hotkeys

Jumps to a context node that have the same parent and the same type. For example, a property and a field are both children of a class but they have a different syntax kind (one is PropertyDeclarationSyntax and another one is FieldDeclarationSyntax). That is why when a class has only fields and properties, and the cursor is set on a field, jump-next hotkey will only cycle between fields.

#### Jump To Next/Previous Sibling Hotkeys

Jumps to a context node that has the same parent but a different type. For example, a property and a field are both children of a class but they have a different syntax kind (one is PropertyDeclarationSyntax and another one is FieldDeclarationSyntax). That is why when a class has only fields and properties, jump-to-next-sibling hotkey will cycle between the first field and the first property in the same class.

#### Jump To Up/Down Hotkeys

Jumps to a parent/child context node. For example, if the cursor is set on a method name (part of the MethodDeclarationSyntax), then jump up will jump to a containing class because it is the parent of the method. In contrast, jumping down will set a cursor on the first child node (parameter list) of the method. To jump to a method body from a method declaration you have to jump to a parameter list and then use "jump-to-next-sibling" hotkey to jump to a method body block, and then use "jump-down" hotkey one more time to navigate to the first statement in the method body.