# Document Creator

[![Build Status](https://sctutton.visualstudio.com/DocumentCreator/_apis/build/status/stutton.DocumentCreator)](https://sctutton.visualstudio.com/DocumentCreator/_build/latest?definitionId=6)

Document Creator is a Windows app for creating test documentation for work items in Azure DevOps or TFS. It allows you to easily create step-by-step instructions with screen shots and images and automatically upload them to Azure DevOps or TFS. A variety of automations can be run including setting fields on work items or saving a copy of the document locally.

## Getting Started

### Download and Install

1. Download the latest release (Setup.exe or Setup.msi) from the [release page](https://github.com/stutton/DocumentCreator/releases/latest).
2. Run the exe or msi
3. Document Creator will start automatically once the installation is complete

### Connect To Azure DevOps or TFS

The first time Document Creator is run it will prompt you to enter your Azure DevOps or TFS server URL (e.g. dev.azure.com/mycompany). You will then be prompted to authenticate with your Microsoft account.

### Create a Template

In order to start creating documents, you need to setup a document template. Document Creator uses Word documents (.docx) with placeholder text that will be replaced by user entered information or fields from work items. Placeholder text looks like this: {FIELD_NAME}. Create a Word document that includes some placeholder fields and save it in an accessible location.

Back in Document Creator, hover over the add (+) button and select New Template (*). On the Details step, enter a Name and Description. Enter or browse to your template Word document. Choose a name that any generated documents will have.

> Note: {ID} is the only placeholder that can be used in the File Name field. It will be replaced with the selected work item ID. For example, TestDocument_{ID}.

On the Query step, add one or more query statements that will be used to determine eligible work items to select from when creating a document using this template. Here are some useful examples:

- Only show PBIs
  - Field: Work Item Type
  - Operator: Equals
  - Value: Product Backlog Item
- Only show items in the Committed state
  - Field: State
  - Operator: Equals
  - Value: Committed

> Note: Work item selection is automatically limited to items assigned to you.

On the Fields step, define the placeholder fields that should be replaced in your template Word document. You can select from the following replacements types:

* Date: The current date
* Name: The current users full name
* Text: Prompts the user for a value during document creation
* Work Item Field: The value of the selected work item field
* List: Appends a list of text and images, created when using the template, to the document. If Use Placeholder is selected, the list will be inserted at the placeholder location in the document

On the Automations tab, you can choose actions that will run when a document is created. You can choose from:

- Attach to Work Item: The document will be uploaded to Azure DevOps or TFS and attached to the selected work item
- Set Work Item Field: Sets the value of a field on the selected work item
- Set Child Work Item Field: Sets the value of a field on all of the selected work item's children
- Save As: Save a copy of the document to the selected folder

Finally, you can review the template's configuration on the Finish step. Click Finish to create the template.

### Create a Document

Now that you have a document template, you can use it to create a document. Your template should be displayed as a card on the main page. Click the card to start a new document.

On the Work Item step, select a work item to link to this document. If the work item you want to select is not listed, you can search for it's ID using the search bar.

On the fields step, you will see a list of all placeholder replacements. Any replacements that require input will be editable (e.g. List and Text fields). The list field will allow you to add steps with (optional) screenshots of a running window or from the clipboard.

Finally, the Finish step will show you what automations will take place when the document is created. Click Finish to create the document.

> Note: At any time you can save your work without creating the document by click the Save icon in the upper right.