# RoslyJump

RoslyJump is a free extension for Visual Studio 2019 for mouseless code navigation. For now, it only has limited support of C# 8.0.

> Important: It only works in C# Editor (no F#, C++, VB support yet).

Build Status:
[![Build Status](https://sapehin.visualstudio.com/RoslyJump/_apis/build/status/psxvoid.RoslyJump?branchName=main)](https://sapehin.visualstudio.com/RoslyJump/_build/latest?definitionId=4&branchName=main)

## License

All code in this project is covered under the Apache 2 license. You can find a copy of it inside the same folder inside License.txt.

## Default Hotkeys

<kbd>Alt</kbd>+<kbd>N</kbd> - jump next

<kbd>Alt</kbd>+<kbd>P</kbd> - jump previous

<kbd>Alt</kbd>+<kbd>U</kbd> - jump up

<kbd>Alt</kbd>+<kbd>D</kbd> - jump down

<kbd>Alt</kbd>+<kbd>K</kbd> - jump next sibling

<kbd>Alt</kbd>+<kbd>J</kbd> - jump previous sibling

## QA

### Where can I find usage examples?

Usage examples can be found [here](https://github.com/psxvoid/RoslyJump/readme-examples.md).

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

### Can I set the default hotkeys to a different one?

Yes. Go to `Tools > Options > Environment > Keyboard`. In the "Show commands containing" type "RoslyJump" you'll see all commands supported by the extension and be able to configure them..

#### Alternative hotkey configuration ideas

If you are not using `Reattach to process` hotkey, then this may be the most convenient (only four main keys two remember):

```xml
<Shortcut Command="RoslyJump.ContextJumpNext" Scope="C# Editor">Alt+N</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpPrev" Scope="C# Editor">Alt+P</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpDown" Scope="C# Editor">Alt+U</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpUp" Scope="C# Editor">Alt+D</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpNextSibling" Scope="C# Editor">Shift+Alt+N</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpPrevSibling" Scope="C# Editor">Shift+Alt+P</Shortcut>
<RemoveShortcut Command="Debug.ReattachtoProcess" Scope="Global">Shift+Alt+P</RemoveShortcut>
```

vim-like:

```xml
<Shortcut Command="RoslyJump.ContextJumpNext" Scope="C# Editor">Alt+L</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpPrev" Scope="C# Editor">Alt+H</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpDown" Scope="C# Editor">Alt+J</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpUp" Scope="C# Editor">Alt+K</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpNextSibling" Scope="C# Editor">Shift+Alt+L</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpPrevSibling" Scope="C# Editor">Shift+Alt+H</Shortcut>
<RemoveShortcut Command="Edit.JoinLines" Scope="Text Editor">Shift+Alt+L, Shift+Alt+J</RemoveShortcut>
<RemoveShortcut Command="Edit.SortLines" Scope="Text Editor">Shift+Alt+L, Shift+Alt+S</RemoveShortcut>
```

wasd-like:

```xml
<Shortcut Command="RoslyJump.ContextJumpNext" Scope="C# Editor">Alt+D</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpPrev" Scope="C# Editor">Alt+A</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpUp" Scope="C# Editor">Alt+W</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpDown" Scope="C# Editor">Alt+S</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpNextSibling" Scope="C# Editor">Shift+Alt+D</Shortcut>
<Shortcut Command="RoslyJump.ContextJumpPrevSibling" Scope="C# Editor">Shift+Alt+A</Shortcut>
<Shortcut Command="Project.AddExistingItem" Scope="Solution Explorer">Shift+Alt+A</Shortcut>
<RemoveShortcut Command="Project.AddExistingItem" Scope="Global">Shift+Alt+A</RemoveShortcut>
```
