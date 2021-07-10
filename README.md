Welcome to Cassie’s Timesaver!

##PLEASE SEE the Welcome to CT.docx file in this repo for the version of this with images.

1. What is Cassie’s Timesaver?	
2. Getting Started	
3. Overview	
4. The Workspace	
5. The Page Editor	
6. Actions	
7. Expressions	
8. Field Reader	

Table of Contents:

-What is Cassie’s Timesaver?
Do you find yourself spending a groan-worthy amount of time filling out paperwork on a word processor like Microsoft Word, or Apache OpenOffice?  If you said yes, then Cassie’s Timesaver can probably help you. If you said no, the rest of us envy you. Cassie’s Timesaver is an application (currently available for Windows only) that can provide you with a simple and streamlined interface to your .docx document templates. The interface, plus a few more bells and whistles working behind the scenes, can dramatically decrease your overall data entry time.
 
Have you ever seen those little brightly colored sticky tags that say, “Sign Here” placed all over a document like a contract? Cassie’s Timesaver uses its own kind of tags inside of documents so it knows where it needs to fill in the right information. These tags will go where you want your information to go. Instead of using predefined tags like “Sign Here”, you name your own tags so they’re meaningful to you. In addition to filling in the blanks, CT can also perform a bit of problem solving using the data you give it. It can do things like, perform calculations, combine bits of text, and do conditional checks to determine what actions need to happen within your document. 

The coup de grace to your data entry woes is CT’s capability to read information from a variety of file types directly into itself. It alleviates the chore of tediously copying and pasting information from another file into your documents. This is especially nice when you have a lot of little pieces of information you need to get in the right place accurately. 

We’ll talk more about these topics below.

-Getting started

	Minimum Requirements	Recommended Requirements
Operating System	Windows XP	Windows 7 (Windows 8 not tested)
Memory	1024mb	2048mb
CPU	1.7gHz	2.4gHz dual core
Free Disk Space	50mb	500mb

-Overview
The Workspace: Here is where you’ll begin. From The Workspace you can load and save a collection of Input Pages called a Book. Input Pages provide a place to enter Information, and can be edited and created in the Page Designer. The Page Designer is accessible from The Workspace. Once you get your document templates and pages all set up and working together, The Workspace is where you’ll enter your data and generate new documents using a pre-existing .docx Template.
-Page Designer: From here, you can specify the layout and optionally the behavior of your Input Pages. Within The Page Editor you’ll find the visual portion of your page on the left. On the top-right, you’ll find 3 tabs labeled, “Page Design”, “Actions”, and “Field Reader”. The Page Design tab contains tools to add to and edit the visual form portion of your page, as well as name the tags that will be placed inside your document template. The Actions tab provides a space to create custom Actions that will occur during document generation. The Field Reader tab provides a space to craft and test Regular Expressions for the purpose of reading text directly from an outside file and into the visual portion of an Input Page.
-Expression Editor: Here you can define the conditional logic for or result of an Action. Don’t let that description sound too complex. It’s really just an over-glorified calculator—you’ll see.

-The Workspace
 
The Workspace (shown above) is the home screen in Cassie’s Timesaver. From here, you can import one or more Input Pages, fill in the necessary information, target a .docx template, and generate your desired document. If your Input Pages are already set up for you, this will be the only screen you need.

Input Pages:

Input Pages provide a form-like interface for your document templates. You can learn more detailed information about Input Pages in the section for the Page Designer. If you right-click an Input Page you’ll bring up a context menu for that Page.
Input Page Context Menu (Right-Click Input Page):
Read Fields: 	If the Input Page has been set up in the Page Designer to Read Fields, you can select a file to have it do so. Another way to activate the Read Fields capability is to drag and drop a file onto the Input Page that you would like to have read it.

Save As: 	Save this Input Page to a .pag file.

Edit in Designer:	Opens this Input Page for editing in the Page Designer.

Clear Fields: 	Empties the Input Page’s Text Fields, and Check Boxes, and sets all Date Dickers to the default date.

Remove Page:	Removes this Input Page from the current Book.


Left and Right Page Panels:

These two panels are containers for your Input Pages.

Input Page Tab Label:

You can click, drag, and drop this label onto another Page Panel to move it there.

Status Bar:

The Status Bar reports which .docx Template (if any) is targeted for use in generating a document. To the right of the targeted template, minor messages will sometimes be displayed in orange.

The Drop-Down Menu: 
File


	New	Clears all Input Pages from the Left and Right Panels.
Save	Saves .bok (Book) of the current collection of Input Pages.
Open	Opens a .bok (Book) - a collection of Input Pages.
Exit	Exits Cassie’s Timesaver.
	

Input
	Import	Imports a .pag (Input Page).
Designer	Opens the Page Designer Window.
(Varies)	A reflection of short-cuts from CT’s Input Folder
Folder	Opens  ‘/My Documents/Cassies Timesaver/Input/’
	

Templates	Select	Target a .docx (Template) via a file dialog box.
Dynamic	(Displays a reflection of short-cuts from CT’s Template folder)
Folder	Opens  ‘/My Documents/Cassies Timesaver/Template/’
	

Output	Generate	Generates your document.
Folder	Opens  ‘/My Documents/Cassies Timesaver/Output/’
	
	

One useful to understand feature of the Input and Template drop-down menus, is that they a portion of them reflect the file/directory structure contained in your Cassies Timesaver folder in your My Documents folder. So for instance, the Input drop-down menu will reflect any .pag and .bok files and folders contained in your computer’s “/My Documents/Cassies Timesaver/Input/” folder. Any changes to that folder on your computer are immediately reflected in the drop-down menu. This sort of folder reflection is also true for Template, only that drop-down menu reflects .docx files instead of .pag and .bok files.
-The Page Designer
 
Drop-Down Menu:
File


	New Page	Creates blank Input Page.

Load Page	Loads a .pag file from your computer.

Revert	Attempts to restore the original state of the current Input Page. If a reference to the original state is unavailable the option to Load a page will appear.

Save 	Save the current Input Page using its previous filename. If the original filename is not available, the Save As dialog will be displayed.

Save As	Choose a name and location to save this Input Page for later use.

Close Designer	Closes the Designer. Any unsaved changes will be lost.
	

Edit
	Undo (CTRL-Z)	Attempts to return the current Input Page to its last state before a change was made.

Redo (CTRL-Y)	Attempts to return the current Input Page to its last state before an Undo was performed.

Cut (CTRL-X)	Removes any selected Items from the Input Page and makes them available in memory for the Paste capability.

Copy (CTRL-C)	Makes the selected Items available in memory for the Paste capability.

Paste (CTRL-V)	Creates a clone of the last Copy/Cut. The position of the copy will be left aligned beneath to lowermost Item on the page. If the short-cut key combination is used, the pasted Items will be placed at the mouse cursor’s location.

Select all	Selects every Item on the current Input Page.
	

Send To Workspace	Attempts to transfer the current Input Page from the Design Editor to the Workspace. Duplicate Tags are not allowed. The Input Page will not be sent if any Field/Action Tags match with any of the Tags in any of the Input Pages in the Workspace. 


Items:
An Item is any selectable component inside your Input Page. Specifically any selectable: Label, Single or Multi-line Text Box, Combo Box, Date Picker, Check Box, Radio Button, or Panel is an Item. Most Items can be made to display their Auto-Label. Label Items do not have Auto-Labels. Auto-Labels themselves are not selectable Items.
Items are further divided into two groups. Items possessing Tags are referred to as Fields. Most Items are Fields. The only two non-Field Items are Labels, and Panels. Field Items contain some type of Information. Most Field Items will contain modifiable Text. The Check Box, and Radio Button are the exceptions which do not display editable Text. For those, only two values are possible: Checked equals true, and unchecked equals false. 
The Information within a Field Item is accessible through its Tag. Valid Tags placed within an Expression or document Template are replaced with the Information contained in the Tag’s corresponding Field.
Items originate from the Drag and Drop Tool Box in the Page Design tab.
Items
 Type	Description
Label	A positionable label. Has no Auto-Label. Non-Field

Single-line Text Box	Accepts Text. Horizontally resizable. Field.

Multi-Line Text Box	Accepts Text. Horizontally and vertically resizable. Field.

Combo Box	A list of selectable text items, editable from Combo Box Edit. Field.

Date Picker	A calendar for selecting a date. Field.

Check Box	A container insensitive switch. Field. 

Radio Button	A container sensitive switch. Only one Radio Button can be marked per Container. Field.

Panel	A positionable Container for Items. Non-Field.

Input Page	A non-positionable Container for Items. Non-Field.


The Editable Space:
The leftmost rectangle in the Page Designer represents the area where you can design the visual content of your Input Page. In this area, components called Items can be positioned and modified. The Editable Space of the Input Page extends below what is displayed on-screen, and can be scrolled into view using the scrollbar to the right of the Editable Space, or by scrolling the mouse wheel.

The Input Page’s Background Color and Tab Label can be changed by left clicking the Editable Space, and changing the desired Item Properties.

Selecting:
Selecting an Item can be done by left clicking an individual Item. Multiple Items can be selected by drawing a Selection Box. A Selection Box is drawn by holding down the left mouse button and dragging the mouse. Releasing the left mouse button selects any Items within the area of the drawn Selection Box. To draw a Selection Box within a Panel, you must first hold down the CTRL button.
Positioning:
Positioning a selected Item can be done by holding the left mouse button and dragging. Releasing the mouse button over the Editable Space will place the Item inside the top-most Container containing the mouse cursor. Items can also be finely positioned by using the keyboard’s arrow keys while holding the CTRL key. A third way to position an Item is by modifying the Top or Left values within the Item Properties box. 
Resizing:
Resizing is possible with all Items except for the Label, Check Box, and Radio Button. If the mouse cursor changes to an orientation of two-way arrows when hovering on the edge of an Item, the Item is drag-resizable in the directions of the arrows. Another way to resize an Item is via the Height and Width values of the Item Properties box.

Drag and Drop Tool Box:
This is your source for adding new Items into an Input Page. New Items can be added to the Input Page by left-clicking the Item’s image in the Drag and Drop Tool Box, and dragging it into the Input Page.

Item Properties:
Selected Items can be modified using the Item Properties box.
Items
 Property	Description
Tag	A unique name or series of characters used to access a Field Item’s Information.

Copy Selected Tags	Copies a list of the selected Field Item’s Tags separated by spaces.

Top	The distance in pixels between an Item’s top edge and the Input Page’s top edge.

Left	The distance in pixels between an Item’s left edge and the Input Page’s left edge.

Height	The Height in pixels of an Item.

Width	The Width in pixels of an Item.

Font	Opens the Font dialog box. Sets the Font of an Item or Auto-Label.

Text Color	Opens the Color dialog box. Sets the Text color of an Item or Auto-Label.

Back Color	Opens the Color dialog box. Sets the Background color of an Item or Auto-Label.

Label Text	Sets the Text of a Label or Auto-Label.

Label Position	Select from a list of predefined positions for Auto-Labels.

Combo Edit Box	Edit the list of a Combo Box. One entry per line.

Prevent Mouse Drag	Prevents unintended Item repositioning.


Copy Selected Tags:
This button gives you a simple way to get your Tags into your documents. Clicking it will copy the Tags of any selected Field Items. Each Tag copied this way will be separated by a space.

Auto-Label:
An Auto-Label is a Label positioned relatively around an Item. It is not a Label Item. All Items except for a Label Item are capable of producing an Auto-Label. An Auto-Label’s position can is changed using the Label Position selector in the Item Properties box. The Text, Font, Text Color, and Back Color can be changed in the Item Properties box.

Status Bar:
Displays minor messages to you.

-Actions
 
Selected Action:
Each Input Page can contain a list of Actions. The Selected Action shows which Action Tag is currently selected. It can be dropped down to select a different Action if others exist. When an Action Tag is selected from the list, its structure is displayed in the Action Logic Tree view.

Copy Action Tag:
Clicking this button will make the Tag signature of the selected Action available to paste as text into other programs.  The signature of an Action Tag depends on the Action’s Type. You’ll learn more about an Action’s Type in the section about the Action Edit dialog.

New / Delete Action:
Left-clicking New Action creates an empty Action, and brings up the Action Edit dialog for that Action. Left-clicking the Delete Action button removes the selected Action from the Input Page currently being edited.

Action Logic Tree:
An Action Logic Tree displays the logical flow of an Action. An Action is something that will be done during your document’s generation. The orange button represents the tree’s Root. The Root is where the Action is given a Tag name, an Action Type and optionally a label. An Action’s Action Type determines what will happen when the Action’s Tag has been placed in a document. Different Action Types do different Actions such as, place a calculation into the document, or toggle the hiding of a document region. You’ll learn more about an Action’s Type in the section about the Action Edit dialog.
The Action Logic Tree has only 2 types of branches, discussed in detail below. They are the If Conditional Expression, and the Result Expression. All Actions must end with a Result. An Expression is just like a math expression. The Expression is discussed in depth in the section about the Expression Editor. 
When a new Action Logic Tree is created, it has two buttons—a blue If button, and a green Result button. These two buttons are called a Fork. Forks are empty, and contain no Expressions. Either button can be left clicked to open up the Expression Editor. Once a Fork is given an Expression in the Expression Editor, it expands to become a full If/Else or Result. To remove an If, Else If or Result branch, simply right click on the branch you wish to remove. Any child branches are removed when the parent branch is removed. It is not possible to remove an Else branch by itself. 

If, Else If, Else:
The blue If, Else If, Else branches are used to guide an Action to evaluate the next Conditional If or Result Expression. At each junction of If, or Else If, a Conditional Expression within is evaluated. A Conditional Expression must evaluate to either true or false. Instructions on creating Conditional Expressions can be found in the section on the Expression Editor.
If a Conditional Expression evaluates to true, then the evaluation proceeds down that branch. If the Expression evaluates to false, then the evaluation moves to the next Else If branch assuming one exists. An Else If branch can be added by left-clicking the Else branch, and adding a Conditional Expression to it. 
In the case that every Conditional Expression starting at If has evaluated to false, evaluation will finally proceed down the Else branch. 
When a new If, Else If, or Else is created, two buttons called a Fork are created as well. A Fork can become an If, or a Result branch. This provides for the capability of nesting Conditional Expressions within Conditional Expressions—if you so choose.

Result:
A green Result branch determines the actual outcome of an Action when its Expression is evaluated. Instructions on creating Result Expressions can be found in the section on the Expression Editor.

Action Result:
The Action Result text box on the bottom of the Actions Tab shows how the current Action currently evaluates. When an Action evaluation produces an error, that error is shown as the result. Errors can come from Conditional, or Result Expressions. An error will also result when the Action attempts to evaluate an empty Fork. A Fork is junction which contains no Expression. During document generation, Action Types such as the Fill Action will replace all instances of the Action Tag with the result of an Expression. This means that any errors in a Fill Action will be made visible in the locations where that Action’s Tag appears in the document Template. You can learn about creating valid Expressions in the section on the Expression Editor.

