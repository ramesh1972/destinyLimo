import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Store } from '@ngrx/store';
import { take } from 'rxjs/operators';
import { Actions, ofType } from '@ngrx/effects';

// CoreUI Modules
import { ButtonModule, DropdownModule } from '@coreui/angular';
import { CardModule } from '@coreui/angular';
import { FormModule, FormCheckComponent } from '@coreui/angular';
import { ButtonDirective } from '@coreui/angular';

import { invokeMaterialTextFetchAPI,materialTextFetchAPI_Success } from '@src/store/actions/material.action';
import { selectMaterialCategorys, selectMaterialTexts } from '@src/store/selectors/material.selector';

import { AccordianParentComponent } from '@src/components/common/components/accordian-parent/accordian-parent.component';

@Component({
  selector: 'app-text',
  standalone: true,
  imports: [CommonModule, FormModule, FormCheckComponent, ButtonDirective, DropdownModule,
    FormsModule,
    ButtonModule,
    CardModule,
    FormModule,
    AccordianParentComponent],
  templateUrl: './text.component.html',
  styleUrl: './text.component.css'
})
export class TextComponent {
  constructor(private readonly store: Store, private actions$: Actions) {
  }

  categories: any[] = []; 
  text: any[] = [];
  newPostString = 'Add Post';
  selectedCategoryName = '';

  ngOnInit() {

    // set data
    this.store.select(selectMaterialCategorys).subscribe((cats) => {
      console.log("material fetch dispatched");
    
      this.categories = [...cats];
    });

    this.store.dispatch((invokeMaterialTextFetchAPI()));


    // Wait for the action to complete
    this.actions$.pipe(
      ofType(materialTextFetchAPI_Success),
      take(1)
    ).subscribe(() => {
      console.log("content fetch dispatched");

      this.store.select(selectMaterialTexts).subscribe((data: any) => {
        console.log('material fetched', data);

        this.text = data.map((mcq: any) => {
          const catName = this.categories.find((cat: any) => cat.id === mcq.material_category_id)?.category_name;
          console.log('catName', catName);
          return {
            ...mcq,
            category_name: catName,
            editing: false,
            adding: false
          };
        });
      });
    });

    
  }

  adjustTextareaHeight(event: Event): void {
    console.log('adjustTextareaHeight');
    const textarea = event.target as HTMLTextAreaElement;
    textarea.style.height = 'auto';
    textarea.style.height = `${textarea.scrollHeight}px`;
  }

  
  getCurrentCategoryName(mcq: any): string {
    return this.selectedCategoryName || mcq.category_name || 'Select a Category';
  }

  selectCategory(categoryName: string, index: number): void {
    console.log('selectCategory', categoryName, index);
    this.selectedCategoryName = categoryName;
    this.text[index].category_name = categoryName; // Update the mcq object if needed
  }


  onAdd() {
    console.log('component onAdd');

    this.selectedCategoryName = '';

    this.text.unshift({
      title: 'Course Title',
      description: '',
      category_type_id: 5,
      adding: true,
      editing: false,
      is_active: true,
      is_public: true,
      is_deleted: false
    });

    this.text.forEach((item: any, index: number) => {
      item.editing = false;
    });
  }

  onEdit(i: number) {
    console.log('component edit', i);

    this.selectedCategoryName = '';

    this.text.forEach((item: any, index: number) => {
      item.editing = false;
      item.adding = false;
    });

    this.text[i].editing = true;
    this.text[i].adding = false;
  }

  onDelete(i: number) {
    this.text[i].is_deleted = true;

    this.selectedCategoryName = '';
    //this.store.dispatch(invokeUpdateContentAPI(this.text[i]));

    // remove from the list array
    this.text.splice(i, 1);

    this.text[i].editing = false;
    this.text[i].adding = false;
  }

  onSave(i: number) {
    console.log('component save', i);

    this.selectedCategoryName = '';

    /*  if (this.text[i].adding) {
       this.store.dispatch(invokeContentCreateAPI({
         content: {
           Id: -1,
           content_type_id: this.text[i].content_type_id,
           title: this.text[i].title,
           description: this.text[i].description,
           is_public: this.text[i].is_public,
           is_active: this.text[i].is_active,
           is_deleted: this.text[i].is_deleted
         }
       }));
     } else if (this.text[i].editing) {
       this.store.dispatch(invokeUpdateContentAPI({
         content: {
           Id: this.text[i].id || -1,
           content_type_id: this.text[i].content_type_id,
           title: this.text[i].title,
           description: this.text[i].description,
           is_public: this.text[i].is_public,
           is_active: this.text[i].is_active,
           is_deleted: this.text[i].is_deleted
         }
       }));
     }
  */
    this.text[i].editing = false;
    this.text[i].adding = false;
  }

  onCancel(i: number) {
    console.log('component cancel', i);
    if (this.text[i].adding) {
      this.text.splice(i, 1);
    }
    this.selectedCategoryName = '';

    this.text[i].editing = false;
    this.text[i].adding = false;
  }
}
