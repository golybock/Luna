export interface LightPageView{
	id: string;
	title: string;
	emoji?: string;
	icon?: string;
	cover?: string;
	childPages: LightPageView[];
}