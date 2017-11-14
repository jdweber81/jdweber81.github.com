import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import {
    FileUploadModule,
    DataTableModule,
    SharedModule
} from 'primeng/primeng';

import { ICustomVisionResponse } from './customVisionResponse';

@Component({
    selector: 'home',
    templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
    showPredictedImage: boolean;
    PredictedImage: string;
    CustomVisionResponse: ICustomVisionResponse;

    constructor() { }

    ngOnInit(): void {
        this.showPredictedImage = false;
        this.PredictedImage = "";
    }

    public onSelect(event) {
        // Set current image
        this.PredictedImage = event.files[0].objectURL;

        // We don't want to show PredictedImage now
        this.showPredictedImage = false;
    }

    public onUpload(event) {
        // The .Net controller will return the predicted results
        // as xhr.responseText - convert it to CustomVisionResponse
        this.CustomVisionResponse = JSON.parse(event.xhr.responseText);

        // We now want to show the PredictedImage and the results
        this.showPredictedImage = true;
    }
    public showAddnextStep() {

    }
}

