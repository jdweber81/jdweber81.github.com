export interface ICustomVisionResponse {
    id: string;
    project: string;
    iteration: string;
    created: string;
    predictions: IPrediction[];
}

export interface IPrediction {
    tagId: string;
    tag: string;
    probability: string;
    link: string;
}