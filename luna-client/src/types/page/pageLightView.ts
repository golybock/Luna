export interface IPageLightView{
	id: string;
	title: string;
	emoji?: string;
	childPages: IPageLightView[];
}