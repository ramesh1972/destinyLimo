import { Component } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { filter, max, take } from 'rxjs/operators';
import { Actions, ofType } from '@ngrx/effects';
import { CommonModule } from '@angular/common';

import * as VTable from '@visactor/vtable';
import { ButtonDirective } from '@coreui/angular';
import { FormCheckLabelDirective } from '@coreui/angular';
import { ButtonGroupComponent } from '@coreui/angular';
import { ReactiveFormsModule, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup } from '@angular/forms';
import { FormCheckComponent } from '@coreui/angular';

import { invokeMaterialMCQFetchAPI, materialMCQFetchAPI_Success } from '@src/store/actions/material.action';
import { selectMaterialCategorys, selectMaterialMCQs } from '@src/store/selectors/material.selector';

import { invokeUserProfilesFetchAPI, UserProfilesFetchAPI_Success } from '@src/store/actions/user.action';
import { invokeUserExamsFetchAPI, UserExamsFetchAPI_Success } from '@src/store/actions/exam.action';
import { selectUserExamsById } from '@src/store/selectors/exam.selector';

import { invokeUserExamByIdFetchAPI, UserExamByIdFetchAPI_Success } from '@src/store/actions/exam.action';
import { selectExamAnswers } from '@src/store/selectors/exam.selector';

import { DataGridComponentHelper } from '@src/components/common/components/grid-parent/data-grid.helper';
import { UserProfile } from '@src/store/models/UserProfile';

interface UserExamsInfo {
  user: UserProfile,
  num_exams_not_taken: number,
  new_exam_assigned_by_admin: boolean
}

@Component({
  selector: 'app-user-exams',
  standalone: true,
  imports: [CommonModule, FormCheckComponent, ButtonDirective, FormCheckLabelDirective,
    ButtonGroupComponent, ReactiveFormsModule,],
  templateUrl: './new-exams.component.html',
  styleUrl: './new-exams.component.css'
})
export class NewExamsComponent {
  constructor(private readonly store: Store, private actions$: Actions) {
    // setup the data grid helper
    this.dataGridHelper = new DataGridComponentHelper(this, this.store);
  }

  formRadio1 = new UntypedFormGroup({
    radio1: new UntypedFormControl('All')
  });

  setRadioValue(value: string): void {
    this.formRadio1.setValue({ radio1: value });

    if (value === 'All') {
      this.dataGridHelper?.table?.updateFilterRules([]);
    }
    else if (value === 'HasExams') {
      this.dataGridHelper?.table?.updateFilterRules([{
        filterFunc: (record: Record<string, any>) => {

          return record["num_exams_not_taken"] > 0;
        }
      }]);
    }
    else if (value === 'HasNoExams') {
      this.dataGridHelper?.table?.updateFilterRules([{
        filterFunc: (record: Record<string, any>) => {

        return record["num_exams_not_taken"] == 0;
        }
      }]);
    }
  }

  dataGridHelper?: DataGridComponentHelper;

  saveButtonEnabled = false;
  resetButtonEnabled = false;

  exams: any[] = [];
  users: any[] = [];
  userExamsInfo: UserExamsInfo[] = [];

  ngOnInit() {
    console.log('file component initialized');

    this.dataGridHelper?.setTableInfo('user_exams', "exam_id", false);

    // set columns
    this.dataGridHelper?.setColumns(this.getColDefs());

    // set data
    this.store.dispatch(invokeUserExamsFetchAPI({ onlyExamsNotStarted: true }));
    this.actions$.pipe(
      ofType(UserExamsFetchAPI_Success),
      take(1)
    ).subscribe((data: any) => {
      console.log("exams fetch dispatched", data);

      this.exams = data.allExams.map((exam: any) => {

        var user = this.users.find((user: any) => user.userId === exam.userId);

        if (user !== undefined && user !== null)
          console.log("found user", user);
        return {
          ...exam,
          userFullName: user ? user.firstName + " " + user.lastName : "Not Found",
          avatar: user ? user.avatar : 'images/avatars/blank-avatar.webp',
        };
      });

      console.log("modified files :", this.exams)

      // users
      this.store.dispatch(invokeUserProfilesFetchAPI());

      this.actions$.pipe(
        ofType(UserProfilesFetchAPI_Success),
        take(1)
      ).subscribe((data: any) => {
        console.log("users fetch dispatched", data);

        this.userExamsInfo = data.allUserProfiles.map((user: UserProfile) => {
          return {
            ...user,
            userFullName: user ? user.firstName + " " + user.lastName : "Not Found",
            avatar: user ? user.avatar : 'images/avatars/blank-avatar.webp',
            num_exams_not_taken: this.exams.filter((exam: any) => exam.userId === user.userId)?.length,
            new_exam_assigned_by_admin: false
          }
        });

        this.dataGridHelper!.setData(this.userExamsInfo);

        // ----> draw the table
        this.drawVTable();
      });
    });
  }

  // columns definition
  // columns definition
  getColDefs() {
    return [
      {
        title: "Learner", field: "userId",
        cellType: 'text',
        width: 90,
        maxWidth: 180,
        sort: true,
        textStick: true,
        style: {
          bgColor: 'rgba(184, 132, 132, 0.3)',
          textBaseline: "middle"
        },
        customLayout: (args: any) => {
          const { table, row, col, rect } = args;
          const record = table.getCellOriginRecord(col, row);
          const { height, width } = rect ?? table.getCellRect(col, row);
          const percentCalc = VTable.CustomLayout.percentCalc;

          const container = new VTable.CustomLayout.Group({
            height,
            width,
            display: 'flex',
            flexDirection: 'row',
            flexWrap: 'nowrap',
            alignItems: 'center',
            boundsPadding: [10, 10, 10, 10],
          });

          const containerLeft: any = new VTable.CustomLayout.Group({
            height: 50,
            width: 190,

            display: 'flex',
            flexDirection: 'row',
            background: 'transparent',
            boundsPadding: [10, 10],
            cornerRadius: 6,
            alignItems: 'flex-start',
          });

          container.add(containerLeft);

          const avatar = new VTable.CustomLayout.Image({
            src: record.avatar,
            width: 40,
            height: 40,
            boundsPadding: [4, 4],
            cornerRadius: 20,
          });

          containerLeft.add(avatar);

          const userName = new VTable.CustomLayout.Text({
            text: record.userFullName,
            fontSize: 13,
            fontFamily: 'sans-serif',
            fill: 'black',
            lineHeight: 20,
            boundsPadding: [4, 10],
            marginTop: 15
          });

          containerLeft.add(userName);

          return {
            rootContainer: container,
            renderDefault: false
          };
        }
      },
      {
        title: "Pending Exams", field: "num_exams_not_taken",
        cellType: 'text',
        width: 150,
        maxWidth: 150,
        columnResizeMode: 'none',
        disableColumnResize: true,
        style: {
          bgColor: 'white',
          textBaseline: "middle",
          selfAlign: 'center',
          textAlign: 'center'
        },
      },
      {
        title: "Assign New Exam", field: "new_exam_assigned_by_admin",
        cellType: 'checkbox',
        width: 160,
        maxWidth: 160,
        columnResizeMode: 'none',
        disableColumnResize: true,
        style: {
          bgColor: 'lightblue',
          textBaseline: "middle",
          textAlign: 'center'
        }
      }
    ];
  }

  getTableOptions() {

    const container = document.getElementById('tableContainer');

    // add delete column to the end
    const option = {
      container: container!,
      columns: this.dataGridHelper?.columns,

      widthMode: 'adaptive',
      heightMode: 'autoHeight',
      autoFillWidth: true,
      autoFillHeight: false,

      animationAppear: {
        duration: 100,
        delay: 50,
        type: 'one-by-one', // all
        direction: 'row' // colunm
      },

      theme: VTable.themes.ARCO
    };

    option.theme = VTable.themes.ARCO.extends({
      underlayBackgroundColor: 'transparent',
      defaultStyle: {
        autoWrapText: true,
        color: 'black',
        textBaseline: "top",
      },
      headerStyle: {
        bgColor: '#a881e1',
        borderColor: '#a881e1',
        borderLineWidth: .3,
        color: 'white',
        lineHeight: 40,
        textBaseline: "middle",
        autoWrapText: false
      },
      bodyStyle: {
        bgColor: 'white',
        borderColor: 'black',
        borderLineWidth: .3,
        color: 'black',
        textBaseline: "middle",
        autoWrapText: true
      },
      checkboxStyle: { defaultFill: 'grey' },
      frameStyle: { cornerRadius: 6, borderColor: 'black', borderLineWidth: .5, shadowColor: 'rgba(0,0,0,.6)', shadowBlur: 3, shadowOffsetX: 1.5, shadowOffsetY: 1.5 },
    });

    return option;
  }

  // styling and drawing the table
  drawVTable() {

    const option = this.getTableOptions();

    // create table
    this.dataGridHelper?.createTable(option);

    this.dataGridHelper?.table?.on('click_cell', args => {
      console.log('change_cell_value for new assign', args);

      const { col, row } = args;

      const record = this.dataGridHelper?.table?.getRecordByCell(col, row);
      const changedValue = this.dataGridHelper?.table?.getCellValue(col, row);

      if (changedValue === true) {
        console.log("adding new assignment");
        // TODO
        // add a new assignment
        // disable this cell
      }
    });
  }

  // user exam answer
}